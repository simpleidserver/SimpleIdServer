https://play.openpolicyagent.org/µ

C:\Projects\SimpleIdServer\src\Authzen\SimpleIdServer.Authzen.Startup\bin\Debug\net8.0\download\opa_windows_amd64.exe build -t wasm -e users/read/allow policies/users/read.rego -o read.wasm

 : pour comiler en WASM

Il existe plusieurs technologies pour évaluer les accès.

# Rego

Appeler l'API qui est exposé, il est pour le moment difficile d'interpréter le code.

Compiler le code "rego" en WASM.


# XACML

Interpréter le fichier.

# CASBIN

Pas d'API exposée pour évaluer une politique, il faut donc interpréter le fichier.

# CEDAR - AWS

Soit la politique CEDAR est évaluée via le service REST de amazon, soit la politique CEDAR est évaluée en interprétant le fichier.

Idée générale, pour les différents langages, il faut :
* Pouvoir évaluer la politique d'autorisation.

Problématique, comment la politique est initiallement structurée ?

```
{
  "subject": {
    "type": "user",
    "id": "alice@acmecorp.com"
  },
  "resource": {
    "type": "account",
    "id": "123"
  },
  "action": {
    "name": "can_read",
    "properties": {
      "method": "GET"
    }
  },
  "context": {
    "time": "1985-10-26T01:22-07:00"
  }
}
```

XACML :

```
<Policy PolicyId="account-read-policy"
        RuleCombiningAlgId="urn:oasis:names:tc:xacml:1.0:rule-combining-algorithm:permit-overrides"
        Version="1.0"
        xmlns="urn:oasis:names:tc:xacml:3.0:core:schema:wd-17">

    <Target>
        <AnyOf>
            <AllOf>
                <!-- Sujet = alice@acmecorp.com -->
                <Match MatchId="urn:oasis:names:tc:xacml:1.0:function:string-equal">
                    <AttributeValue DataType="http://www.w3.org/2001/XMLSchema#string">alice@acmecorp.com</AttributeValue>
                    <AttributeDesignator AttributeId="subject-id"
                                         Category="urn:oasis:names:tc:xacml:1.0:subject-category:access-subject"
                                         DataType="http://www.w3.org/2001/XMLSchema#string"/>
                </Match>
            </AllOf>
        </AnyOf>
    </Target>

    <Rule RuleId="allow-account-read" Effect="Permit">
        <Target>
            <AnyOf>
                <AllOf>
                    <!-- Ressource = account 123 -->
                    <Match MatchId="urn:oasis:names:tc:xacml:1.0:function:string-equal">
                        <AttributeValue DataType="http://www.w3.org/2001/XMLSchema#string">account</AttributeValue>
                        <AttributeDesignator AttributeId="resource-type"
                                             Category="urn:oasis:names:tc:xacml:3.0:attribute-category:resource"
                                             DataType="http://www.w3.org/2001/XMLSchema#string"/>
                    </Match>
                    <Match MatchId="urn:oasis:names:tc:xacml:1.0:function:string-equal">
                        <AttributeValue DataType="http://www.w3.org/2001/XMLSchema#string">123</AttributeValue>
                        <AttributeDesignator AttributeId="resource-id"
                                             Category="urn:oasis:names:tc:xacml:3.0:attribute-category:resource"
                                             DataType="http://www.w3.org/2001/XMLSchema#string"/>
                    </Match>

                    <!-- Action = can_read -->
                    <Match MatchId="urn:oasis:names:tc:xacml:1.0:function:string-equal">
                        <AttributeValue DataType="http://www.w3.org/2001/XMLSchema#string">can_read</AttributeValue>
                        <AttributeDesignator AttributeId="action-id"
                                             Category="urn:oasis:names:tc:xacml:3.0:attribute-category:action"
                                             DataType="http://www.w3.org/2001/XMLSchema#string"/>
                    </Match>

                    <!-- Action.method = GET -->
                    <Match MatchId="urn:oasis:names:tc:xacml:1.0:function:string-equal">
                        <AttributeValue DataType="http://www.w3.org/2001/XMLSchema#string">GET</AttributeValue>
                        <AttributeDesignator AttributeId="method"
                                             Category="urn:oasis:names:tc:xacml:3.0:attribute-category:action"
                                             DataType="http://www.w3.org/2001/XMLSchema#string"/>
                    </Match>
                </AllOf>
            </AnyOf>
        </Target>

        <!-- Condition temporelle optionnelle (ex. contexte) -->
        <Condition>
            <Apply FunctionId="urn:oasis:names:tc:xacml:1.0:function:dateTime-greater-than">
                <AttributeDesignator AttributeId="current-time"
                                     Category="urn:oasis:names:tc:xacml:3.0:attribute-category:environment"
                                     DataType="http://www.w3.org/2001/XMLSchema#dateTime"/>
                <AttributeValue DataType="http://www.w3.org/2001/XMLSchema#dateTime">1985-10-26T01:00:00-07:00</AttributeValue>
            </Apply>
        </Condition>

    </Rule>
</Policy>
```

REGO :

```
package authz

default allow = false

allow {
    input.subject.id == "alice@acmecorp.com"
    input.resource.type == "account"
    input.resource.id == "123"
    input.action.name == "can_read"
    input.action.properties.method == "GET"

    # Contexte temporel : doit être après 1h00
    time_after("1985-10-26T01:00:00-07:00", input.context.time)
}

# Comparaison temporelle
time_after(threshold, value) = true {
    t1 := time.parse_rfc3339_ns(threshold)
    t2 := time.parse_rfc3339_ns(value)
    t2 > t1
}
```

CASBIN :

```
[request_definition]
r = sub, obj, act, method

[policy_definition]
p = sub, obj, act, method

[policy_effect]
e = some(where (p.eft == allow))

[matchers]
m = r.sub == p.sub &&
    r.obj == p.obj &&
    r.act == p.act &&
    r.method == p.method
```

# CEDAR - AWS

```
{
  "@context": "https://cedarpolicy.com/context/cedar/v1",
  "@type": "Policy",
  "description": "Allow alice to perform can_read with GET on account 123",
  "target": {
    "principal": {
      "@type": "User",
      "id": "alice@acmecorp.com"
    },
    "action": {
      "name": "can_read",
      "properties": {
        "method": "GET"
      }
    },
    "resource": {
      "@type": "Account",
      "id": "123"
    }
  },
  "condition": {
    "type": "DateTimeGreaterThan",
    "arguments": [
      {
        "type": "Variable",
        "value": "environment.time"
      },
      {
        "type": "DateTimeLiteral",
        "value": "1985-10-26T01:00:00-07:00"
      }
    ]
  },
  "effect": "Allow"
}
```

