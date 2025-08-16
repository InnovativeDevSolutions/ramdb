#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Get the length of a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to get
 *
 * Return Value:
 * Length of the hash <NUMBER>
 *
 * Example:
 * ["myKey"] call ramdb_db_fnc_hlen; (Server or Singleplayer only)
 * ["myKey"] remoteExec ["ramdb_db_fnc_hlen", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [["_key", "", [""]]];

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["hlen", [_key]];

GVAR(inuse) = false;

_result
