#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Increment a field in a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to increment
 * 1: Field <STRING> - Field to increment
 * 2: Increment <NUMBER> - Increment value
 *
 * Return Value:
 * New value of the key after increment <NUMBER>
 *
 * Example:
 * ["myKey", "myField", 1] call ramdb_db_fnc_hincrby; (Server or Singleplayer only)
 * ["myKey", "myField", 1] remoteExec ["ramdb_db_fnc_hincrby", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_field", "", [""]],
    ["_increment", 0, [0]]
];

if (_field isEqualTo "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hincrby' Invalid Input for Field '%1'", _field];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["hincrby", [_key, _field, _increment]];

GVAR(inuse) = false;

_result
