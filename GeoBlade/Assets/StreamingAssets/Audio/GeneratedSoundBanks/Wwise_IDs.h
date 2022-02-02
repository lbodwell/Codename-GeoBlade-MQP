/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID DIALOGUE_TRIGGER = 4029211690U;
        static const AkUniqueID LEVEL1_MUSIC = 2026953958U;
        static const AkUniqueID PLAYER_ATTACK = 2824512041U;
        static const AkUniqueID PLAYER_FOOTSTEP = 2453392179U;
        static const AkUniqueID PLAYER_JUMP = 1305133589U;
        static const AkUniqueID PLAYER_UNSHEATHE = 917509252U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace ATTACK_TYPE
        {
            static const AkUniqueID GROUP = 2578485808U;

            namespace STATE
            {
                static const AkUniqueID HEAVY = 2732489590U;
                static const AkUniqueID LIGHT1 = 3192784680U;
                static const AkUniqueID LIGHT2 = 3192784683U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace ATTACK_TYPE

        namespace DIALOGUE_LINE
        {
            static const AkUniqueID GROUP = 3247544628U;

            namespace STATE
            {
                static const AkUniqueID LVL1_STASIS_ROOM_IRIS_01 = 1590806114U;
                static const AkUniqueID LVL1_STASIS_ROOM_IRIS_02 = 1590806113U;
                static const AkUniqueID LVL1_STASIS_ROOM_IRIS_03 = 1590806112U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DIALOGUE_LINE

        namespace FOOTSTEP_TYPE
        {
            static const AkUniqueID GROUP = 2615620554U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID RUN = 712161704U;
                static const AkUniqueID WALK = 2108779966U;
            } // namespace STATE
        } // namespace FOOTSTEP_TYPE

        namespace MATERIAL
        {
            static const AkUniqueID GROUP = 3865314626U;

            namespace STATE
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace MATERIAL

        namespace MUSIC_LEVEL1_TRACK1
        {
            static const AkUniqueID GROUP = 2489350135U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID SECTION1 = 3524678539U;
                static const AkUniqueID SECTION2 = 3524678536U;
                static const AkUniqueID SECTION3 = 3524678537U;
            } // namespace STATE
        } // namespace MUSIC_LEVEL1_TRACK1

    } // namespace STATES

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID DIALOGUE = 3930136735U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PLAYER = 1069431850U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
