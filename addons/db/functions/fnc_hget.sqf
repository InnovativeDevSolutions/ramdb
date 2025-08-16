#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get a field from a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 * 1: Field <STRING> - Field to get
 * 2: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 * 3: Function <STRING> - Function to call
 * 4: Call <BOOL> - Run in unscheduled environment
 *
 * Return Value:
 * None
 *
 * Example:
 * ["myKey", "myField"] call ramdb_db_fnc_hget; (Server or Singleplayer only)
 * ["myKey", "myField", myObject, "myFunction", true] remoteExec ["ramdb_db_fnc_hget", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_field", "", [""]],
    ["_obj", "", [[], "", 0, grpNull, objNull, sideUnknown]],
    ["_fnc", "", [""]],
    ["_call", false, [false]]
];

if (_field isEqualTo "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hget' Invalid Input for Field '%1'", _field];
};

DB_COMMON(_key,[ARR_5(QUOTE(hget),[ARR_1(_field)],_obj,_fnc,_call)]);
