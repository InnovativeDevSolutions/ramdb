#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Increment a field in a hash by a float.
 *
 * Arguments:
 * 0: Key <STRING>
 * 1: Field <STRING> - Field to increment
 * 2: Increment <NUMBER> - Increment value
 *
 * Return Value:
 * New value of the key after increment <NUMBER>
 *
 * Example:
 * ["myKey", "myField", 1.0] call ramdb_db_fnc_hincrbyfloat; (Server or Singleplayer only)
 * ["myKey", "myField", 1.0] remoteExec ["ramdb_db_fnc_hincrbyfloat", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_field", "", [""]],
    ["_increment", 0.0, [0]]
];

if (_field isEqualTo "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hincrbyfloat' Invalid Input for Field '%1'", _field];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["hincrbyfloat", [_key, _field, _increment]];

GVAR(inuse) = false;

_result
