using System.Text;
using System.Text.Json;
using Wasmtime;

namespace SimpleIdServer.Authzen.Rego.Eval;

public partial class WasmOpaPolicy
{
	private int _dataAddr;
	private int _baseHeapPtr;
	private int _dataHeapPtr;

	private Linker _linker;
	private Store _store;
	private Memory _envMemory;
	private Instance _instance;

	private Dictionary<string, Object> _registeredBuiltins = new Dictionary<string, object>();

	public IReadOnlyDictionary<string, int> Entrypoints { get; private set; }
	public IReadOnlyDictionary<int, string> Builtins { get; private set; }

	public int? AbiVersion { get; private set; }
	public int? AbiMinorVersion { get; private set; }

	internal WasmOpaPolicy(Engine engine, Module module, long minMemSize)
	{
		_linker = new Linker(engine);
		_store = new Store(engine);
		_envMemory = new Memory(_store, minMemSize);

		LinkImports();

		Initialize(module);
	}
    
	private void LinkImports()
    {
        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.MemoryName, _envMemory);

        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.Abort, Function.FromCallback(_store,
            (Caller caller, int addr) =>
            {
                string info = _envMemory.ReadNullTerminatedString(addr);
                throw new Exception(info);
            })
        );

        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.Builtin0, Function.FromCallback(_store,
            (Caller caller, int builtinId, int opaCtxReserved) =>
            {
                return CallBuiltin(builtinId, opaCtxReserved);
            })
        );

        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.Builtin1, Function.FromCallback(_store,
            (Caller caller, int builtinId, int opaCtxReserved, int addr1) =>
            {
                return CallBuiltin(builtinId, opaCtxReserved, addr1);
            })
        );

        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.Builtin2, Function.FromCallback(_store,
            (Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2) =>
            {
                return CallBuiltin(builtinId, opaCtxReserved, addr1, addr2);
            })
        );

        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.Builtin3, Function.FromCallback(_store,
            (Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3) =>
            {
                return CallBuiltin(builtinId, opaCtxReserved, addr1, addr2, addr3);
            })
        );

        _linker.Define(WasmOpaConstants.Module, WasmOpaConstants.Builtin4, Function.FromCallback(_store,
            (Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4) =>
            {
                return CallBuiltin(builtinId, opaCtxReserved, addr1, addr2, addr3, addr4);
            })
        );
    }

	private void Initialize(Module module)
	{
		_instance = _linker.Instantiate(_store, module);

		string builtins = DumpJson(Policy_Builtins());
		Builtins = ParseBuiltinsJson(builtins);

		_dataAddr = LoadJson("{}");
		_baseHeapPtr = Policy_opa_heap_ptr_get();
		_dataHeapPtr = _baseHeapPtr;

		string entrypoints = DumpJson(Policy_Entrypoints());
		Entrypoints = ParseEntryPointsJson(entrypoints);

		ReadAbiVersionGlobals();
	}

	private Dictionary<string, int> ParseEntryPointsJson(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json, GetSTJDefaultOptions());

		var dict = new Dictionary<string, int>();
		foreach (JsonProperty prop in document.RootElement.EnumerateObject())
		{
			dict.Add(prop.Name, prop.Value.GetInt32());
		}

		return dict;
	}

	private Dictionary<int, string> ParseBuiltinsJson(string json)
	{
		if ("{}" == json) return new Dictionary<int, string>();

		using JsonDocument document = JsonDocument.Parse(json, GetSTJDefaultOptions());

		var dict = new Dictionary<int, string>();
		foreach (JsonProperty prop in document.RootElement.EnumerateObject())
		{
			dict.Add(prop.Value.GetInt32(), prop.Name);
		}

		return dict;
	}

	private JsonDocumentOptions GetSTJDefaultOptions()
	{
		return new JsonDocumentOptions
		{
			AllowTrailingCommas = true
		};
	}

	private void ReadAbiVersionGlobals()
	{
		var major = Policy_opa_wasm_abi_version();
		if (major.HasValue)
		{
			if (major != 1)
			{
				throw new BadImageFormatException($"{major} ABI version is unsupported");
			}

			AbiVersion = major;
		}
		else
		{
			// opa_wasm_abi_version undefined
		}

		var minor = Policy_opa_wasm_abi_minor_version();
		if (minor.HasValue)
		{
			AbiMinorVersion = minor;
		}
		else
		{
			// opa_wasm_abi_minor_version undefined
		}
	}

	public string EvaluateJson(string json, bool disableFastEvaluate = false)
	{
		return ExecuteEvaluate(json, null, disableFastEvaluate);
	}

	public string EvaluateJson(string json, string entrypoint, bool disableFastEvaluate = false)
	{
		bool found = Entrypoints.TryGetValue(entrypoint, out var epId);
		return ExecuteEvaluate(json, epId, disableFastEvaluate);
	}

	private string ExecuteEvaluate(string json, int? entrypoint, bool disableFastEvaluate)
	{
		if (!disableFastEvaluate && (AbiMinorVersion.HasValue && AbiMinorVersion.Value >= 2))
		{
			return FastEvaluate(json, entrypoint);
		}

		// Reset the heap pointer before each evaluation
		Policy_opa_heap_ptr_set(_dataHeapPtr);

		// Load the input data
		int inputAddr = LoadJson(json);

		// Setup the evaluation context
		int ctxAddr = Policy_opa_eval_ctx_new();
		Policy_opa_eval_ctx_set_input(ctxAddr, inputAddr);
		Policy_opa_eval_ctx_set_data(ctxAddr, _dataAddr);

		if (entrypoint.HasValue)
		{
			Policy_opa_eval_ctx_set_entrypoint(ctxAddr, entrypoint.Value);
		}

		// Actually evaluate the policy
		Policy_eval(ctxAddr);

		// Retrieve the result
		int resultAddr = Policy_opa_eval_ctx_get_result(ctxAddr);
		return DumpJson(resultAddr);
	}

	private string FastEvaluate(string json, int? entrypoint)
	{
		if (!entrypoint.HasValue) entrypoint = 0; // use default entry point

		int jsonLength = Encoding.UTF8.GetByteCount(json);
		_envMemory.WriteString(_dataHeapPtr, json);  // UTF8 is the default in WriteString when no encoding is passed

		int resultaddr = Policy_opa_eval(entrypoint.Value, _dataAddr, _dataHeapPtr, jsonLength, _dataHeapPtr + jsonLength);

		return _envMemory.ReadNullTerminatedString(resultaddr);
	}

	public void SetDataJson(string json)
	{
		Policy_opa_heap_ptr_set(_baseHeapPtr);
		_dataAddr = LoadJson(json);
		_dataHeapPtr = Policy_opa_heap_ptr_get();
	}

	private int LoadJson(string json)
	{
		int jsonLength = Encoding.UTF8.GetByteCount(json);
		int addr = Policy_opa_malloc(jsonLength);
		_envMemory.WriteString(addr, json);
		int parseAddr = Policy_opa_json_parse(addr, jsonLength);
		if (0 == parseAddr)
		{
			throw new ArgumentNullException("Parsing failed");
		}

		return parseAddr;
	}

	private string DumpJson(int addrResult)
	{
		int addr = Policy_opa_json_dump(addrResult);

		// Note: ReadNullTerminatedString internally returns Encoding.UTF8.GetString(...) - ReadString takes Encoding param
		return _envMemory.ReadNullTerminatedString(addr);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_envMemory = null;
			_instance = null;
			_store.Dispose();
			_store = null;
			_linker.Dispose();
			_linker = null;
			_registeredBuiltins.Clear();
		}
	}

	public void RegisterSdkBuiltins()
	{
		// See https://github.com/christophwille/dotnet-opa-wasm/issues/3#issuecomment-957579119
		// Wire-up here, implementations to go into OpaPolicy.SdkBuiltins.cs
		throw new NotImplementedException();
	}

	public void RegisterBuiltin<TResult>(string name, Func<TResult> callback)
	{
		Func<int> builtinCallback = () =>
		{
			var result = callback();
			return BuiltinResultToAddress(result);
		};

		_registeredBuiltins.Add(name, builtinCallback);
	}

	public void RegisterBuiltin<TArg1, TResult>(string name, Func<TArg1, TResult> callback)
	{
		Func<int, int> builtinCallback = (addr1) =>
		{
			var result = callback(
				BuiltinArgToValue<TArg1>(addr1));

			return BuiltinResultToAddress(result);
		};

		_registeredBuiltins.Add(name, builtinCallback);
	}

	public void RegisterBuiltin<TArg1, TArg2, TResult>(string name, Func<TArg1, TArg2, TResult> callback)
	{
		Func<int, int, int> builtinCallback = (addr1, addr2) =>
		{
			var result = callback(
				BuiltinArgToValue<TArg1>(addr1),
				BuiltinArgToValue<TArg2>(addr2));

			return BuiltinResultToAddress(result);
		};

		_registeredBuiltins.Add(name, builtinCallback);
	}

	public void RegisterBuiltin<TArg1, TArg2, TArg3, TResult>(string name, Func<TArg1, TArg2, TArg3, TResult> callback)
	{
		Func<int, int, int, int> builtinCallback = (addr1, addr2, addr3) =>
		{
			var result = callback(
				BuiltinArgToValue<TArg1>(addr1),
				BuiltinArgToValue<TArg2>(addr2),
				BuiltinArgToValue<TArg3>(addr3));

			return BuiltinResultToAddress(result);
		};

		_registeredBuiltins.Add(name, builtinCallback);
	}

	public void RegisterBuiltin<TArg1, TArg2, TArg3, TArg4, TResult>(string name, Func<TArg1, TArg2, TArg3, TArg4, TResult> callback)
	{
		Func<int, int, int, int, int> builtinCallback = (addr1, addr2, addr3, addr4) =>
		{
			var result = callback(
				BuiltinArgToValue<TArg1>(addr1),
				BuiltinArgToValue<TArg2>(addr2),
				BuiltinArgToValue<TArg3>(addr3),
				BuiltinArgToValue<TArg4>(addr4));

			return BuiltinResultToAddress(result);
		};

		_registeredBuiltins.Add(name, builtinCallback);
	}

	private int CallBuiltin(int builtinId, int opaCtxReserved)
	{
		return ((Func<int>)GetFuncForBuiltinId(builtinId))();
	}

	private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1)
	{
		return ((Func<int, int>)GetFuncForBuiltinId(builtinId))(addr1);
	}

	private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1, int addr2)
	{
		return ((Func<int, int, int>)GetFuncForBuiltinId(builtinId))(addr1, addr2);
	}

	private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3)
	{
		return ((Func<int, int, int, int>)GetFuncForBuiltinId(builtinId))(addr1, addr2, addr3);
	}

	private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4)
	{
		return ((Func<int, int, int, int, int>)GetFuncForBuiltinId(builtinId))(addr1, addr2, addr3, addr4);
	}

    private object GetFuncForBuiltinId(int builtinId)
    {
        return 1;
	}

	private T BuiltinArgToValue<T>(int addr)
	{
		var json = DumpJson(addr);
		return JsonSerializer.Deserialize<T>(json);
	}

	private int BuiltinResultToAddress(object result)
	{
		var json = JsonSerializer.Serialize(result);
		return LoadJson(json);
	}
}
