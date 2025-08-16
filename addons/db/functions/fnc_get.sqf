#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get a key from the database.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 * 1: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 2: Function <STRING> - Function to call
 * 3: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * Value of the key <ANY>
 *
 * Example:
 * ["myKey"] call ramdb_db_fnc_get; (Server or Singleplayer only)
 * ["myKey", myObject, "ramdb_db_fnc_test", true] call ramdb_db_fnc_get; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_get' Invalid Input for Key '%1'", _key];
};

DB_COMMON(_key,[ARR_5(QUOTE(get),[],_obj,_fnc,_call)]);
