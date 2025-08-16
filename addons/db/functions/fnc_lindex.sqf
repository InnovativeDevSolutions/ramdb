#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get a value from a list at a specific index.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 * 1: Index <NUMBER> - Index to get
 * 2: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 3: Function <STRING> - Function to call
 * 4: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * Value at the specified index <ANY>
 *
 * Example:
 * ["myKey", 1, objNull, "", false] call ramdb_db_fnc_lindex; (Server or Singleplayer only)
 * ["myKey", 1, objNull, "", false] remoteExec ["ramdb_db_fnc_lindex", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_index", 0, [0]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_lindex' Invalid Input for Key '%1'", _key];
};

DB_COMMON(_key,[ARR_5(QUOTE(lindex),[ARR_1(_index)],_obj,_fnc,_call)]);
