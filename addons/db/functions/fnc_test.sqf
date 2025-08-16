#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Test function for ArmaRAMDb.
 *
 * Arguments:
 * 0: Data <ANY>
 *
 * Return Value:
 * None
 *
 * Example:
 * ["myData"] call ramdb_db_fnc_test; (Server or Singleplayer only)
 * ["myData"] remoteExec ["ramdb_db_fnc_test", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_test' Data '%1'", _this];

hint str _this;
