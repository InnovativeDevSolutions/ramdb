#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Delete a field from a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to delete
 * 1: Fields <ARRAY> - Fields to delete
 *
 * Return Value:
 * Number of fields deleted <NUMBER>
 *
 * Example:
 * ["myKey", ["myField1", "myField2"]] call ramdb_db_fnc_hdel; (Server or Singleplayer only)
 * ["myKey", ["myField1", "myField2"]] remoteExec ["ramdb_db_fnc_hdel", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_fields", [], [[]]]
];

if (_fields isEqualTo []) exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hdel' Invalid Input for Fields '%1'", _fields];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _args = [_key];
_args append _fields;

private _result = "ArmaRAMDb" callExtension ["hdel", _args];

GVAR(inuse) = false;

_result
