namespace SimpleIdServer.Authzen.Rego.Eval;

public partial class WasmOpaPolicy
{
    private int? Policy_opa_wasm_abi_version()
    {
    	var global = _instance.GetGlobal("opa_wasm_abi_version");
    	return (int?)global?.GetValue();
    }   
    private int? Policy_opa_wasm_abi_minor_version()
    {
    	var global = _instance.GetGlobal("opa_wasm_abi_minor_version");
    	return (int?)global?.GetValue();
    }  
     
    private int Policy_Builtins()
    {
        var run = _instance.GetFunction<int>("builtins");
        return run();
    }  
     
    private int Policy_Entrypoints()
    {
        var run = _instance.GetFunction<int>("entrypoints");
        return run();
    }
       
    private int Policy_opa_heap_ptr_get()
    {
        var run = _instance.GetFunction<int>("opa_heap_ptr_get");
        return run();
    } 
      
    private void Policy_opa_heap_ptr_set(int ptr)
    {
        var run = _instance.GetAction<int>("opa_heap_ptr_set");
        run(ptr);
    }  
     
    private int Policy_opa_eval_ctx_new()
    {
        var run = _instance.GetFunction<int>("opa_eval_ctx_new");
        return run();
    }   
    
    private void Policy_opa_eval_ctx_set_input(int ctxAddr, int inputAddr)
    {
        var run = _instance.GetAction<int, int>("opa_eval_ctx_set_input");
        run(ctxAddr, inputAddr);
    }   
    
    private void Policy_opa_eval_ctx_set_data(int ctxAddr, int dataAddr)
    {
        var run = _instance.GetAction<int, int>("opa_eval_ctx_set_data");
        run(ctxAddr, dataAddr);
    }   
    
    private void Policy_opa_eval_ctx_set_entrypoint(int ctxAddr, int entrypoint)
    {
        var run = _instance.GetAction<int, int>("opa_eval_ctx_set_entrypoint");
        run(ctxAddr, entrypoint);
    }   
    private void Policy_eval(int ctxAddr)
    {
    	var run = _instance.GetFunction<int, int>("eval");
    	_ = run(ctxAddr);
    }   
    private int Policy_opa_eval(/*int addr, */
    	int entrypoint_id, int dataaddr, int jsonaddr, int jsonlength, int heapaddr/*, int format*/)
    {
    	var run = _instance.GetFunction<int, int, int, int, int, int, int, int>("opa_eval");
    	return run(0 /* always 0 */, entrypoint_id,
    		dataaddr, jsonaddr, jsonlength, heapaddr, 0 /* json format */);
    }   
    
    private int Policy_opa_eval_ctx_get_result(int ctxAddr)
    {
        var run = _instance.GetFunction<int, int>("opa_eval_ctx_get_result");
        return run(ctxAddr);
    }   
    
    private int Policy_opa_malloc(int length)
    {
        var run = _instance.GetFunction<int, int>("opa_malloc");
        return run(length);
    }   
    
    private int Policy_opa_json_parse(int addr, int length)
    {
        var run = _instance.GetFunction<int, int, int>("opa_json_parse");
        return run(addr, length);
    }   
    
    private int Policy_opa_json_dump(int addrResult)
    {
        var run = _instance.GetFunction<int, int>("opa_json_dump");
        return run(addrResult);
    }
}
