#if ENABLE_PICO_OPENXR_SDK
using System.Collections.Generic;
using ByteDance.PICO.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif


namespace ByteDance.PICO.OpenXR
{
    /// <summary>
    /// Plane detection (XR_PICO_spatial_plane) for the OpenXR runtime. Creates the plane sense data
    /// provider and the AR Foundation XRPlaneSubsystem (PXR_PlaneSubsystem), which the stock SDK only
    /// does from the PICO-native PXR_Loader.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "PICO Plane Detection",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = "1.0.0",
        FeatureId = featureId)]
#endif
    public class PICOSpatialPlane : OpenXRFeature
    {
        public const string featureId = "com.pico.openxr.feature.spatialplane";
        public const string extensionString = "XR_PICO_spatial_plane XR_PICO_spatial_sensing XR_EXT_future";

        public static bool isEnable => OpenXRRuntime.IsExtensionEnabled("XR_PICO_spatial_plane");

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            PXR_Plugin.MixedReality.UPxr_CreatePlaneDetectionSenseDataProvider();
        }

        protected override void OnSessionExiting(ulong xrSession)
        {
            PXR_MixedReality.GetSenseDataProviderState(PxrSenseDataProviderType.PlaneDetection, out var providerState);
            if (providerState == PxrSenseDataProviderState.Running)
            {
                PXR_MixedReality.StopSenseDataProvider(PxrSenseDataProviderType.PlaneDetection);
            }

            PXR_Plugin.MixedReality.UPxr_DestroySenseDataProvider(
                PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.PlaneDetection));

            base.OnSessionExiting(xrSession);
        }

#if AR_FOUNDATION_5||AR_FOUNDATION_6
        static List<XRPlaneSubsystemDescriptor> planeSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(
                planeSubsystemDescriptors,
                PXR_PlaneSubsystem.k_SubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            StartSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            StopSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            DestroySubsystem<XRPlaneSubsystem>();
        }
#endif
    }
}
#endif
