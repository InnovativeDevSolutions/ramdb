#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get all values from a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 * 1: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 2: Function <STRING> - Function to call
 * 3: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * Array of values in the hash <ARRAY>
 *
 * Example:
 * ["myKey", player, "setVariable", true] call ramdb_db_fnc_hvals; (Server or Singleplayer only)
 * ["myKey", player, "setVariable", true] remoteExec ["ramdb_db_fnc_hvals", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

DB_COMMON(_key,[ARR_5(QUOTE(hvals),[],_obj,_fnc,_call)]);
