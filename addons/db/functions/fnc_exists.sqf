#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Check if a key exists in the database.
 *
 * Arguments:
 * 0: Key <STRING> - Key to check
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey"] call ramdb_db_fnc_exists; (Server or Singleplayer only)
 * ["myKey"] remoteExec ["ramdb_db_fnc_exists", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [["_key", "", [[], ""]]];

if (_key isEqualTo "" || _key isEqualTo []) exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_exists' Invalid Input for Key(s) '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["exists", [_key]];

GVAR(inuse) = false;

_result
