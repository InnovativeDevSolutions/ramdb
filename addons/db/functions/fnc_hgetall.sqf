#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get all fields from a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 * 1: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 2: Function <STRING> - Function to call
 * 3: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * None
 *
 * Example:
 * ["myKey", myObject, "setVariable", true] call ramdb_db_fnc_hgetall; (Server or Singleplayer only)
 * ["myKey", myObject, "setVariable", true] remoteExec ["ramdb_db_fnc_hgetall", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

DB_COMMON(_key,[ARR_5(QUOTE(hgetall),[],_obj,_fnc,_call)]);
