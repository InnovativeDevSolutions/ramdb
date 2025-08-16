#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Insert an element into a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to insert
 * 1: Before <BOOL> - Insert before
 * 2: Reference <STRING> - Reference to insert
 * 3: Data <ARRAY> - Data to insert
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", true, "myReference", ["myData"]] call ramdb_db_fnc_linsert; (Server or Singleplayer only)
 * ["myKey", true, "myReference", ["myData"]] remoteExec ["ramdb_db_fnc_linsert", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_before", false, [false]],
    ["_reference", "", [[], "", 0, false]],
    ["_value", "", [[], "", 0, false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_linsert' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["linsert", [_key, _before, _reference, _value]];

GVAR(inuse) = false;

_result
