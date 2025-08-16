#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Initial Extension settings.
 *
 * Arguments:
 * N/A
 *
 * Return Value:
 * N/A
 *
 * Examples:
 * N/A
 *
 * Public: Yes
 */

private _dll = "ArmaRAMDb" callExtension ["version", []];

diag_log text format ["ArmaRAMDb: DLL Version %1 found", _dll];
diag_log text "ArmaRAMDb: Functions loaded and Initialization completed!";
diag_log text format ["ArmaRAMDb: Buffer size set to %1 Bytes", GVAR(RAMDb_buffer)];
diag_log text "ArmaRAMDb: Ready for use!";
