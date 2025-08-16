#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get the length of a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 *
 * Return Value:
 * Length of the list <NUMBER>
 *
 * Example:
 * ["myKey"] call ramdb_db_fnc_llen; (Server or Singleplayer only)
 * ["myKey"] remoteExec ["ramdb_db_fnc_llen", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [["_key", "", [""]]];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_llen' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["llen", [_key]];

GVAR(inuse) = false;

_result
