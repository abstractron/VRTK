namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class VRTK_SDKInputOverrideType
    {
        [Header("SDK settings")]

        [Tooltip("An optional SDK Setup to use to determine when to modify the transform.")]
        public VRTK_SDKSetup loadedSDKSetup;
        [Tooltip("An optional SDK controller type to use to determine when to modify the transform.")]
        public SDK_BaseController.ControllerType controllerType;
    }

    [Serializable]
    public class VRTK_SDKButtonInputOverrideType : VRTK_SDKInputOverrideType
    {
        [Header("Button Override")]

        [Tooltip("The button to override to.")]
        public VRTK_ControllerEvents.ButtonAlias overrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
    }

    public class VRTK_SDKInputOverride : VRTK_SDKControllerReady
    {
        [Header("Interact Grab")]

        [Tooltip("The Interact Grab script to override the button on.")]
        public VRTK_InteractGrab interactGrabScript;
        [Tooltip("The list of overrides.")]
        public List<VRTK_SDKButtonInputOverrideType> interactGrabOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Interact Use")]

        [Tooltip("The Interact Use script to override the button on.")]
        public VRTK_InteractUse interactUseScript;
        [Tooltip("The list of overrides.")]
        public List<VRTK_SDKButtonInputOverrideType> interactUseOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Pointer")]

        [Tooltip("The Pointer script to override the button on.")]
        public VRTK_Pointer pointerScript;
        [Tooltip("The list of overrides for the activation button.")]
        public List<VRTK_SDKButtonInputOverrideType> pointerActivationOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the selection button.")]
        public List<VRTK_SDKButtonInputOverrideType> pointerSelectionOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        protected override void OnEnable()
        {
            base.OnEnable();
            ManageInputs();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (sdkManager != null && !gameObject.activeSelf)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            }
        }

        protected override void ControllerReady(VRTK_ControllerReference controllerReference)
        {
            if (sdkManager != null && sdkManager.loadedSetup != null && gameObject.activeInHierarchy)
            {
                ManageInputs();
            }
        }

        protected virtual VRTK_SDKButtonInputOverrideType GetSelectedButtonModifier(List<VRTK_SDKButtonInputOverrideType> overrideTypes, VRTK_ControllerReference controllerReference)
        {
            //attempt to find by the overall SDK set up to start with
            VRTK_SDKButtonInputOverrideType selectedModifier = overrideTypes.FirstOrDefault(item => item.loadedSDKSetup == sdkManager.loadedSetup);

            //If no sdk set up is found or it is null then try and find by the SDK controller
            if (selectedModifier == null)
            {
                SDK_BaseController.ControllerType currentControllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                selectedModifier = overrideTypes.FirstOrDefault(item => item.controllerType == currentControllerType);
            }
            return selectedModifier;
        }

        protected virtual VRTK_SDKButtonInputOverrideType GetSelectedAxisModifier(List<VRTK_SDKButtonInputOverrideType> overrideTypes, VRTK_ControllerReference controllerReference)
        {
            //attempt to find by the overall SDK set up to start with
            VRTK_SDKButtonInputOverrideType selectedModifier = overrideTypes.FirstOrDefault(item => item.loadedSDKSetup == sdkManager.loadedSetup);

            //If no sdk set up is found or it is null then try and find by the SDK controller
            if (selectedModifier == null)
            {
                SDK_BaseController.ControllerType currentControllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                selectedModifier = overrideTypes.FirstOrDefault(item => item.controllerType == currentControllerType);
            }
            return selectedModifier;
        }

        protected virtual void ManageInputs()
        {
            ManageInteractGrab();
            ManageInteractUse();
            ManagePointer();
        }

        protected virtual VRTK_ControllerReference GetReferenceFromEvents(VRTK_ControllerEvents controllerEvents)
        {
            return VRTK_ControllerReference.GetControllerReference((controllerEvents != null ? controllerEvents.gameObject : null));
        }

        protected virtual void ManageInteractGrab()
        {
            if (interactGrabScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(interactGrabScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedModifier = GetSelectedButtonModifier(interactGrabOverrides, controllerReference);
                if (selectedModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    interactGrabScript.grabButton = selectedModifier.overrideButton;
                }
            }
        }

        protected virtual void ManageInteractUse()
        {
            if (interactUseScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(interactUseScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedModifier = GetSelectedButtonModifier(interactUseOverrides, controllerReference);
                if (selectedModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    interactUseScript.useButton = selectedModifier.overrideButton;
                }
            }
        }

        protected virtual void ManagePointer()
        {
            if (pointerScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(pointerScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedActivationModifier = GetSelectedButtonModifier(pointerActivationOverrides, controllerReference);
                if (selectedActivationModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    pointerScript.activationButton = selectedActivationModifier.overrideButton;
                }

                VRTK_SDKButtonInputOverrideType selectedSelectionModifier = GetSelectedButtonModifier(pointerSelectionOverrides, controllerReference);
                if (selectedSelectionModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    pointerScript.selectionButton = selectedSelectionModifier.overrideButton;
                }
            }
        }
    }
}