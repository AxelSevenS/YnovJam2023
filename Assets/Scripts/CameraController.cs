using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

using SevenGame.Utility;
    
public class CameraController : MonoBehaviour {

    public Camera camera;
    public UniversalAdditionalCameraData cameraData;

    private Quaternion entityRotation;
    private Vector3 delayedPosition;
    private Vector3 cameraVector;
    [SerializeField]private float distanceToPlayer = 1f; 
    [SerializeField]private float fov = 90f;

    private Vector3 velocity = Vector3.zero;
    private float additionalDistance = 0f;

    [SerializeField] private Character target;


    // private void OnEnable(){
    //     SetCurrent();
    // }

    // private void Reset() => Awake();
    private void Awake() {
        camera = Camera.main;
        cameraData = camera.GetComponent<UniversalAdditionalCameraData>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){


        // if (PlayerEntityController.current?.entity == null) return;

        UpdateCameraRotation();

        UpdateCameraDistance();
    }

    private void FixedUpdate(){
        // if (PlayerEntityController.current?.entity == null) return;

        UpdateCameraPosition();
    }

    private Vector2 mousePosition;
    // private Vector2 mouseDelta;
    private void UpdateCameraRotation() {
        Vector2 mouseDelta = new Vector2( Input.GetAxis("Mouse X") / 2.5f, -Input.GetAxis("Mouse Y") / 2.5f );
        // mousePosition = Input.mousePosition;
        mousePosition = new Vector2( mousePosition.x+mouseDelta.x, Mathf.Clamp(mousePosition.y+mouseDelta.y, -90, 90) );
    }


    private void UpdateCameraDistance(){
        // Vector3 cameraRelativePosition = PlayerEntityController.current.entity.state.cameraPosition;
        Vector3 cameraRelativePosition = new Vector3(1.2f, 0.65f, -1.75f);
        Vector3 cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer -additionalDistance);

        cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 3f * GameUtility.timeDelta);
    }

    private void UpdateCameraPosition(){
        entityRotation = Quaternion.Slerp( entityRotation, target.transform.rotation, 4f * GameUtility.timeDelta );
        
        // transform.rotation = PlayerEntityController.current.worldCameraRotation;

        Quaternion mouseRotation = Quaternion.AngleAxis(mousePosition.x, Vector3.up) * Quaternion.AngleAxis(mousePosition.y, Vector3.right);
        transform.rotation = target.transform.rotation * mouseRotation;

        delayedPosition = Vector3.SmoothDamp(delayedPosition, target.transform.position, ref velocity, 0.06f);
        Vector3 camPosition = transform.rotation * cameraVector;

        float camDistance = camPosition.magnitude;
        float distanceToWall = 0.4f;

        if ( Physics.Raycast( delayedPosition, camPosition, out RaycastHit cameraCollisionHit, camPosition.magnitude + distanceToWall, LayerMask.NameToLayer("default") ) ){

            Vector3 collisionToPlayer = delayedPosition - cameraCollisionHit.point;
            Vector3 collisionTangent = Vector3.up;

            Debug.DrawRay( cameraCollisionHit.point, collisionToPlayer * 3f, Color.red );
            float collisionAngle = 90 - Vector3.Angle( collisionToPlayer.normalized, cameraCollisionHit.normal );

            float camMargin = distanceToWall / Mathf.Sin(collisionAngle * Mathf.Deg2Rad);
            
            camDistance = collisionToPlayer.magnitude - camMargin;
        }

        Vector3 finalPos = delayedPosition + camPosition.normalized * camDistance;
        
        transform.position = finalPos;
    }

}