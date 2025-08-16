#include "\z\ace\addons\main\script_macros.hpp"

#define AFUNC(var1,var2) TRIPLES(DOUBLES(ace,var1),fnc,var2)
#define BFUNC(var1) TRIPLES(BIS,fnc,var1)
#define CFUNC(var1) TRIPLES(CBA,fnc,var1)
#define TFUNC(var1) TRIPLES(TFAR,fnc,var1)

#define PATHTOR_SYS(var1,var2,var3) MAINPREFIX\var1\SUBPREFIX\var2\var3
#define PATHTOR(var1) PATHTOR_SYS(PREFIX,COMPONENT,var1)
#define PATHTOER(var1,var2) PATHTOR_SYS(PREFIX,var1,var2)
#define QPATHTOR(var1) QUOTE(PATHTOR(var1))
#define QPATHTOER(var1,var2) QUOTE(PATHTOER(var1,var2))

#define CLASS(var1) DOUBLES(PREFIX,var1)
#define QCLASS(var1) QUOTE(DOUBLES(PREFIX,var1))

#define DB_COMMON(key,args) \
    args params ["_operation","_params","_obj","_fnc","_call"]; \
    private _data = []; \
    private _response = ""; \
    private _result = ""; \
    private _args = [key]; \
    if (_params isNotEqualTo []) then { \
        _args append _params; \
    }; \
    if (_obj isNotEqualTo "") then { \
        _args append [_fnc, _obj, _call]; \
    }; \
    waitUntil { !GVAR(inuse) }; \
    GVAR(inuse) = true; \
    _response = "ArmaRAMDb" callExtension [_operation, _args]; \
    _result = _response select 0; \
    switch (_result) do { \
        case "NotFound"; \
        case "[]": { \
            diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_%1' Can't find Key '%2'", _operation, _key]; \
        }; \
        case "OK": { \
            diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_%1' Key '%2' is being sent in chunks to external function", _operation, _key]; \
        }; \
        default { \
            _data = (call compile _result); \
            diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_%1' Data '%2'", _operation, _data]; \
        }; \
    }; \
    GVAR(inuse) = false; \
    if (_obj isNotEqualTo "") then { \
        if (_call) then { \
            _data remoteExecCall [_fnc, _obj, false]; \
        } else { \
            _data remoteExec [_fnc, _obj, false]; \
        }; \
    } else { \
        _data \
    }

#ifdef DISABLE_COMPILE_CACHE
    #undef PREP
    #define PREP(fncName) DFUNC(fncName) = compile preprocessFileLineNumbers QPATHTOF(functions\DOUBLES(fnc,fncName).sqf)
#else
    #undef PREP
    #define PREP(fncName) [QPATHTOF(functions\DOUBLES(fnc,fncName).sqf), QFUNC(fncName)] call CBA_fnc_compileFunction
#endif
