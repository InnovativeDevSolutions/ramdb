#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Pop a value from the right of a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to pop
 * 1: Count <NUMBER> - Count to pop
 * 2: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 3: Function <STRING> - Function to call
 * 4: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * Array of popped values <ARRAY>
 *
 * Example:
 * ["myKey", 1] call ramdb_db_fnc_rpop; (Server or Singleplayer only)
 * ["myKey", 1, myObject, "myFunction"] remoteExec ["ramdb_db_fnc_rpop", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_count", 1, [0]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_rpop' Invalid Input for Key '%1'", _key];
};

DB_COMMON(_key,[ARR_5(QUOTE(rpop),[ARR_1(_count)],_obj,_fnc,_call)]);
