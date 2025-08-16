#include "script_component.hpp"

class CfgPatches {
    class ADDON {
        name = COMPONENT_NAME;
        units[] = {};
        weapons[] = {};
        requiredVersion = REQUIRED_VERSION;
        requiredAddons[] = {"cba_main"};
        authors[] = {"J. Schmidt", "Creedcoder"};
        author = "IDSolutions";
        VERSION_CONFIG;
    };
};

#include "CfgEditorCategories.hpp"
#include "CfgEditorSubcategories.hpp"
#include "CfgMods.hpp"
#include "CfgMPGameTypes.hpp"
#include "CfgNotifications.hpp"
