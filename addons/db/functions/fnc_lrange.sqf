#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get a range of values from a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 * 1: Start <NUMBER> - Start index
 * 2: End <NUMBER> - End index
 * 3: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 4: Function <STRING> - Function to call
 * 5: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * Array of values <ARRAY>
 *
 * Example:
 * ["myKey", 1, 10] call ramdb_db_fnc_lrange; (Server or Singleplayer only)
 * ["myKey", 1, 10, myObject, "myFunction"] remoteExec ["ramdb_db_fnc_lrange", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_start", 0, [0]],
    ["_end", -1, [0]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_lrange' Invalid Input for Key '%1'", _key];
};

DB_COMMON(_key,[ARR_5(QUOTE(lrange),[ARR_2(_start,_end)],_obj,_fnc,_call)]);
