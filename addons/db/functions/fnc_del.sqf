#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Delete a key from the database.
 *
 * Arguments:
 * 0: Key <STRING> - Key to delete
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey"] call ramdb_db_fnc_del; (Server or Singleplayer only)
 * ["myKey"] remoteExec ["ramdb_db_fnc_del", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [["_key", "", [[], ""]]];

if (_key isEqualTo "" || _key isEqualTo []) exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_del' Invalid Input for Key(s) '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["del", [_key]];

GVAR(inuse) = false;

_result
