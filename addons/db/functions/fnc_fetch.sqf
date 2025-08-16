#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Handle chunks of data from DB. (Internal use only)
 *
 * Arguments:
 * 0: Unique ID <STRING> - Unique ID of the data
 * 1: Function <STRING> - Function to call
 * 2: Index <NUMBER> - Index of the data
 * 3: Total Index <NUMBER> - Total index of the data
 * 4: Data Chunk <STRING> - Data chunk
 * 5: Call <BOOL> - Run in unscheduled environment
 * 6: Object <OBJECT> - Target to return data can be Object, Array of Objects, Owner ID
 *
 * Return Value:
 * None
 *
 * Example:
 * None
 *
 * Public: No
 */

#ifdef __A3__DEBUG__
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_fetch' Input: %1", _this];
#endif

params ["_uniqueID", "_fnc", "_index", "_total", "_datachunk", "_call", "_obj"];

private _dataString = "";
private _index_array = [];
private _count_total = -1;

ramdb_db_fetch_array pushBackUnique [_uniqueID, _fnc, _index, _total, _datachunk, _call, _obj];

_count_total = {
    if (_uniqueID == _x select 0) then {
        _index_array pushBackUnique [_x select 2, _x select 4];
        true
    } else {
        false
    }
} count ramdb_db_fetch_array;

if (_count_total == _total) then {
    _index_array sort true;
    
    {
        _dataString = _dataString + (_x select 1);
    } forEach _index_array;
    
    #ifdef __A3__DEBUG__
        diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_fetch' Data String: %1", _dataString];
    #endif

    private _data = parseSimpleArray _dataString;

    #ifdef __A3__DEBUG__
        diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_fetch' Data: %1", _data];
    #endif

    if !(isNil "_obj" || _obj isNotEqualTo "" || _obj isNotEqualTo []) then {
        if (_call) then {
            _data remoteExecCall [_fnc, _obj, false];
        } else {
            _data remoteExec [_fnc, _obj, false];
        };
    } else {
        _data;
    };

    ramdb_db_fetch_array = ramdb_db_fetch_array select {!((_x select 0) in [_uniqueID])};
};
