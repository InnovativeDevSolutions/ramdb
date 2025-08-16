#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Set a field in a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to set
 * 1: Field <STRING> - Field to set
 * 2: Data <ARRAY> - Data to set
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", "myField", ["myData"]] call ramdb_db_fnc_hset; (Server or Singleplayer only)
 * ["myKey", "myField", ["myData"]] remoteExec ["ramdb_db_fnc_hset", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_field", "", [""]],
    ["_data", [], [[], "", 0, false]]
];

if (_field isEqualTo "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hset' Invalid Input for Field '%1'", _field];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["hset", [_key, _field, _data]];

GVAR(inuse) = false;

_result
