// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System.Collections;

namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the tap gesture again to place.
    /// This script is used in conjunction with GazeManager, WorldAnchorManager, and SpatialMappingManager.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Interpolator))]
    public class TapToPlaceCustom : MonoBehaviour, IInputClickHandler
    {
        #region Init

        [Tooltip("Distance from camera to keep the object while placing it.")]
        public float DefaultGazeDistance = 2.0f;

        [Tooltip("Place parent on tap instead of current game object.")]
        public bool PlaceParentOnTap;

        [Tooltip("Specify the parent game object to be moved on tap, if the immediate parent is not desired.")]
        public GameObject ParentGameObjectToPlace;

        /// <summary>
        /// Keeps track of if the user is moving the object or not.
        /// Setting this to true will enable the user to move and place the object in the scene.
        /// Useful when you want to place an object immediately.
        /// </summary>
        [Tooltip("Setting this to true will enable the user to move and place the object in the scene without needing to tap on the object. Useful when you want to place an object immediately.")]
        public bool IsBeingPlaced;

        [Tooltip("Setting this to true will allow this behavior to control the DrawMesh property on the spatial mapping.")]
        public bool AllowMeshVisualizationControl = false;

        [Tooltip("Should the center of the Collider be used instead of the gameObjects world transform.")]
        public bool UseColliderCenter;
        private Interpolator interpolator;

        /// <summary>
        /// The default ignore raycast layer built into unity.
        /// </summary>
        private const int IgnoreRaycastLayer = 2;
        private Dictionary<GameObject, int> layerCache = new Dictionary<GameObject, int>();
        private Vector3 PlacementPosOffset;
        public bool propIsInHand = false;

        #endregion

        /// <summary>
        /// Start setup
        /// </summary>
        protected virtual void Start()
        {
            if (PlaceParentOnTap)
            {
                ParentGameObjectToPlace = GetParentToPlace();
                PlaceParentOnTap = ParentGameObjectToPlace != null;
            }

            interpolator = EnsureInterpolator();

            if (IsBeingPlaced)
            {
                StartPlacing();
            }
            else
            {
                /*
                 * If we are not starting out with actively placing the object, give it a World Anchor.
                 */

                AttachWorldAnchor();
            }
        }

        /// <summary>
        /// Setup while enabling
        /// </summary>
        private void OnEnable()
        {
            Bounds bounds = transform.GetColliderBounds();
            PlacementPosOffset = transform.position - bounds.center;
        }

        /// <summary>
        /// Returns the predefined GameObject or the immediate parent when it exists
        /// </summary>
        /// <returns></returns>
        private GameObject GetParentToPlace()
        {
            if (ParentGameObjectToPlace)
            {
                return ParentGameObjectToPlace;
            }
            return gameObject.transform.parent ? gameObject.transform.parent.gameObject : null;
        }

        /// <summary>
        /// Ensures an interpolator on either the parent or on the GameObject itself and returns it.
        /// </summary>
        private Interpolator EnsureInterpolator()
        {
            var interpolatorHolder = PlaceParentOnTap ? ParentGameObjectToPlace : gameObject;
            return interpolatorHolder.EnsureComponent<Interpolator>();
        }

        /// <summary>
        /// A step in the callback cascade
        /// </summary>
        protected virtual void Update()
        {
            if (!IsBeingPlaced)
            {
                return;
            }
            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 placementPosition = GetPlacementPosition(cameraTransform.position, cameraTransform.forward, DefaultGazeDistance);

            if (UseColliderCenter)
            {
                placementPosition += PlacementPosOffset;
            }
            if (PlaceParentOnTap)
            {
                placementPosition = ParentGameObjectToPlace.transform.position + (placementPosition - gameObject.transform.position);
            }

            /*
             * Update the placement to match the user's gaze.
             */

            interpolator.SetTargetPosition(placementPosition);

            // Rotate this object to face the user

            /*
             * Removed the lower functionality, because the Prop always automatically rotates back to zero if 'IsBeingPlaced' = true.
             * This would change the proper rotation setup, which can be changed and stored within the Prop by the user in runtime
             */

            //interpolator.SetTargetRotation(Quaternion.Euler(0, cameraTransform.localEulerAngles.y, 0));
        }

        /// <summary>
        /// Called by a Prop button 
        /// Enables removing the Prop in the scene by using raycast and gaze direction
        /// </summary>
        public void ButtonStartPlacement()
        {
            IsBeingPlaced = true;
            HandlePlacement();
        }

        /// <summary>
        /// Toggle functionality for the replacement activation/deactivation 
        /// </summary>
        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            /*
             * On each tap gesture, toggle whether the user is in placing mode.
             */

            if (IsBeingPlaced)
            {
                IsBeingPlaced = false;
                HandlePlacement();
                eventData.Use();
            }
            if (propIsInHand == true)
            {
                SetPropInHand(false);
            }
        }

        /// <summary>
        /// Starts and stops the replacement
        /// </summary>
        private void HandlePlacement()
        {
            if (IsBeingPlaced)
            {
                StartPlacing();
            }
            else
            {
                StopPlacing();
            }
        }

        /// <summary>
        /// Start replacement for Props
        /// </summary>
        private void StartPlacing()
        {
            SetPropInHand(true);
            ToggleSpatialMesh();
            RemoveWorldAnchor();
        }

        /// <summary>
        /// Stops replacement of Props 
        /// </summary>
        private void StopPlacing()
        {
            SetPropInHand(false);
            ToggleSpatialMesh();
            AttachWorldAnchor();
        }

        /// <summary>
        /// Final steps
        /// </summary>
        IEnumerator WaitForPossibleClick()
        {
            var layerCacheTarget = PlaceParentOnTap ? ParentGameObjectToPlace : gameObject;
            layerCacheTarget.ApplyLayerCacheRecursively(layerCache);
            ToggleSpatialMesh();
            AttachWorldAnchor();
            SetPropInHand(false);
            yield return true;
        }

        /// <summary>
        /// Cares for the World Anchor and set the "SetPropInHand" variable back to false to activate script logic in the "Transparency" script
        /// </summary>
        private void AttachWorldAnchor()
        {
            if (WorldAnchorManager.Instance != null)
            {
                SetPropInHand(false);
                if (GameObject.FindGameObjectWithTag("PanelMenu"))
                {
                    GameObject.FindGameObjectWithTag("PanelMenu").GetComponent<ToggleStoryBoardSubmenuProps>().SetActualPropInHand(null);
                }

                /* 
                 * Add world anchor when object placement is done.
                 * In older Unity Versions Worldanchormanager does not work, so i disabled it for now
                 */

#if UNITY_2018_1_OR_NEWER
                if(PlaceParentOnTap)
                {
                    WorldAnchorManager.Instance.AttachAnchor(ParentGameObjectToPlace, ParentGameObjectToPlace.name);
                }
                else
                {
                    WorldAnchorManager.Instance.AttachAnchor(gameObject, gameObject.name);
                }
#endif
            }
        }

        /// <summary>
        /// Cares for the World Anchor
        /// </summary>
        private void RemoveWorldAnchor()
        {
            if (WorldAnchorManager.Instance != null)
            {
                /* 
                 * Removes existing world anchor if any exist.
                 * In older Unity Versions Worldanchormanager does not work, so i disabled it for now
                 */

#if UNITY_2018_1_OR_NEWER
                  if(PlaceParentOnTap)
                {
                    WorldAnchorManager.Instance.RemoveAnchor(ParentGameObjectToPlace);
                }
                else
                {
                    WorldAnchorManager.Instance.RemoveAnchor(gameObject);
                }
#endif
            }
        }

        /// <summary>
        /// If the user is in placing mode, display the spatial mapping mesh.
        /// </summary>
        private void ToggleSpatialMesh()
        {
            if (SpatialMappingManager.Instance != null && AllowMeshVisualizationControl)
            {
                SpatialMappingManager.Instance.DrawVisualMeshes = IsBeingPlaced;
            }
        }

        /// <summary>
        /// If we're using the spatial mapping, check to see if we got a hit, else use the gaze position.
        /// </summary>
        /// <returns>Placement position in front of the user</returns>
        private static Vector3 GetPlacementPosition(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
        {
            RaycastHit hitInfo;
            if (SpatialMappingRaycast(headPosition, gazeDirection, out hitInfo))
            {
                return hitInfo.point;
            }
            return GetGazePlacementPosition(headPosition, gazeDirection, defaultGazeDistance);
        }

        /// <summary>
        /// Does a raycast on the spatial mapping layer to try to find a hit.
        /// </summary>
        /// <param name="origin">Origin of the raycast</param>
        /// <param name="direction">Direction of the raycast</param>
        /// <param name="spatialMapHit">Result of the raycast when a hit occurred</param>
        private static bool SpatialMappingRaycast(Vector3 origin, Vector3 direction, out RaycastHit spatialMapHit)
        {
            if (SpatialMappingManager.Instance != null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask))
                {
                    spatialMapHit = hitInfo;
                    return true;
                }
            }
            spatialMapHit = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Get placement position either from GazeManager hit or in front of the user as backup
        /// </summary>
        /// <param name="headPosition">Position of the users head</param>
        /// <param name="gazeDirection">Gaze direction of the user</param>
        /// <param name="defaultGazeDistance">Default placement distance in front of the user</param>
        private static Vector3 GetGazePlacementPosition(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
        {
            if (GazeManager.Instance.HitObject != null)
            {
                return GazeManager.Instance.HitPosition;
            }
            return headPosition + gazeDirection * defaultGazeDistance;
        }

        /// <summary>
        /// Sets "propIsInHand" to true.
        /// Causes an action in the Update method of the attached "Transparency" script
        /// </summary>
        public void SetPropInHand(bool setIt)
        {
            propIsInHand = setIt;
        }

        /// <summary>
        /// Returns the state of the "propIsInHand" variable, to check whether the prop must be invisible or not
        /// </summary>
        public bool GetPropInHand()
        {
            return propIsInHand;
        }
    }
}
