// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public static class CollisionUtility {

//     public static bool ColliderCast( this Collider collider, Vector3 position, Vector3 direction, out RaycastHit hit, float skinThickness, LayerMask layerMask ){
//         if ( collider is CapsuleCollider capsule) {

//             capsule.GetCapsuleInfo( position, skinThickness, out Vector3 startPos, out Vector3 endPos, out float radius );
//             return Physics.CapsuleCast( startPos, endPos, radius, direction.normalized, out hit, direction.magnitude + skinThickness, layerMask );
//         }else if ( collider is SphereCollider sphere) {

//             float skinnedRadius = sphere.radius - Mathf.Min(skinThickness, sphere.radius - 0.01f);
//             return Physics.SphereCast( sphere.transform.position, skinnedRadius, direction, out hit, direction.magnitude + skinnedRadius, layerMask );
//         }else if ( collider is BoxCollider box) {
            
//             Vector3 skinThicknessVector = new Vector3(skinThickness, skinThickness, skinThickness);
//             return Physics.BoxCast( box.transform.position, box.size - skinThicknessVector, direction, out hit, box.transform.rotation, 1f, layerMask );
//         }else{
//             hit = new RaycastHit();
//             return false;
//         }
//     }

//     public static Collider[] ColliderOverlap( this Collider collider, Vector3 position, float skinThickness, LayerMask layerMask ){
//         if ( collider is CapsuleCollider capsule ) {

//             capsule.GetCapsuleInfo( position, skinThickness, out Vector3 startPos, out Vector3 endPos, out float radius );
//             return Physics.OverlapCapsule( startPos, endPos, radius, layerMask );
//         }else if ( collider is SphereCollider sphere) {

//             float skinnedRadius = sphere.radius - Mathf.Min(skinThickness, sphere.radius - 0.01f);
//             return Physics.OverlapSphere( sphere.transform.position, skinnedRadius, layerMask );
//         }else if ( collider is BoxCollider box) {
            
//             Vector3 skinThicknessVector = new Vector3(skinThickness, skinThickness, skinThickness);
//             return Physics.OverlapBox( box.transform.position, box.size - skinThicknessVector, box.transform.rotation, layerMask );
//         }else{
//             return new Collider[0];
//         }
//     }

//     public static void GetCapsuleInfo( this CapsuleCollider capsule, Vector3 position, float skinThickness, out Vector3 startPos, out Vector3 endPos, out float radius ){
        
//         Transform capsuleTransform = capsule.transform;
//         skinThickness = Mathf.Max(skinThickness, 0.01f);

//         // depending on the collider's "direction" we set the direction of the capsule length; 
//         // if capsule.direction is 0, the capsule will extend in the collider's right and left directions;
//         Vector3 capsuleDirection;
//         float heightScale;
//         float radiusScale;
//         switch (capsule.direction){
//             default:
//                 capsuleDirection = capsuleTransform.right;
//                 heightScale = capsuleTransform.localScale.x;
//                 radiusScale = Mathf.Max(capsuleTransform.localScale.y, capsuleTransform.localScale.z);
//                 break;
//             case 1:
//                 capsuleDirection = capsuleTransform.up;
//                 heightScale = capsuleTransform.localScale.y;
//                 radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.z);
//                 break;
//             case 2:
//                 capsuleDirection = capsuleTransform.forward;
//                 heightScale = capsuleTransform.localScale.z;
//                 radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.y);
//                 break;
//         }

//         Vector3 capsulePosition = position + capsuleTransform.rotation * Vector3.Scale(capsule.center, capsuleTransform.localScale);
//         float scaledRadius = (capsule.radius * radiusScale);

//         float scaledHalfHeight = capsule.height/2f * heightScale;
//         Vector3 capsuleHalf = (scaledHalfHeight - scaledRadius) * capsuleDirection;

//         startPos = capsulePosition + capsuleHalf; 
//         endPos = capsulePosition - capsuleHalf;
//         radius = scaledRadius - skinThickness;
//     }

//     // public static bool SkinnedCapsuleCast( this CapsuleCollider capsule, Vector3 position, Vector3 direction, float skinThickness, out RaycastHit hit, LayerMask layerMask ){


//     //     Transform capsuleTransform = capsule.transform;
//     //     skinThickness = Mathf.Max(skinThickness, 0.01f);

//     //     // depending on the collider's "direction" we set the direction of the capsule length; 
//     //     // if capsule.direction is 0, the capsule will extend in the collider's right and left directions;
//     //     Vector3 capsuleDirection;
//     //     float heightScale;
//     //     float radiusScale;
//     //     switch (capsule.direction){
//     //         default:
//     //             capsuleDirection = capsuleTransform.right;
//     //             heightScale = capsuleTransform.localScale.x;
//     //             radiusScale = Mathf.Max(capsuleTransform.localScale.y, capsuleTransform.localScale.z);
//     //             break;
//     //         case 1:
//     //             capsuleDirection = capsuleTransform.up;
//     //             heightScale = capsuleTransform.localScale.y;
//     //             radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.z);
//     //             break;
//     //         case 2:
//     //             capsuleDirection = capsuleTransform.forward;
//     //             heightScale = capsuleTransform.localScale.z;
//     //             radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.y);
//     //             break;
//     //     }

//     //     Vector3 capsulePosition = position + capsuleTransform.rotation * Vector3.Scale(capsule.center, capsuleTransform.localScale);
//     //     float scaledRadius = (capsule.radius * radiusScale);

//     //     float scaledHalfHeight = capsule.height/2f * heightScale;
//     //     Vector3 capsuleHalf = (scaledHalfHeight - scaledRadius) * capsuleDirection;

//     //     Vector3 startPos = capsulePosition + capsuleHalf; 
//     //     Vector3 endPos = capsulePosition - capsuleHalf;
//     //     float skinnedRadius = scaledRadius - skinThickness;

//     //     // Debug.DrawLine(startPos, endPos, Color.red);
//     //     // Debug.DrawLine(startPos, startPos + capsuleDirection * skinnedRadius, Color.blue);
//     //     // Debug.DrawLine(endPos, endPos - capsuleDirection * skinnedRadius, Color.blue);

//     //     var result = Physics.CapsuleCast(startPos, endPos, skinnedRadius, direction.normalized, out hit, direction.magnitude + skinThickness, layerMask);

//     //     return result;
//     // }

//     public static Vector3 GetSize( this Collider collider ){
//         if ( collider is CapsuleCollider capsule){
//             Vector3[] directionArray = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
//             Vector3 result = new Vector3();
//             for (int i = 0; i < 3; i ++) {
//                 if (i == capsule.direction)
//                     result += directionArray[i] * capsule.height;
//                 else
//                     result += directionArray[i] * capsule.radius * 2;
//             }
//             return result;
//         }else if ( collider is BoxCollider box){
//             return box.size;
//         }else if ( collider is SphereCollider sphere){
//             return new Vector3(sphere.radius, sphere.radius, sphere.radius);
//         }
//         return Vector3.zero;
//     }

//     public static Vector3 GetCenter( this Collider collider ){
//         if ( collider is CapsuleCollider capsule)
//             return capsule.center;
//         else if ( collider is BoxCollider box)
//             return box.center;
//         else if ( collider is SphereCollider sphere)
//             return sphere.center;
//         return Vector3.zero;
//     }

//     // public static bool ColliderGroundCheck( this Collider collider, out RaycastHit hit, float skinThickness = 0.15f ){
//     //     if (collider is CapsuleCollider capsule){
            
//     //         Vector3 colliderPosition = collider.transform.position + collider.transform.rotation * capsule.center;
//     //         float skinnedRadius = capsule.radius - Mathf.Min(skinThickness, capsule.radius - 0.01f);

//     //         float castLength = capsule.height/2f + 0.1f;

//     //         return Physics.SphereCast( colliderPosition, skinnedRadius, -collider.transform.up * castLength, out hit, castLength, Global.GroundMask );
//     //     }else if (collider is BoxCollider box){
//     //         return Physics.BoxCast(box.transform.position, box.size - new Vector3(skinThickness, skinThickness, skinThickness), -collider.transform.up, out hit, box.transform.rotation, 1f, Global.GroundMask);
//     //     }else{
//     //         return false;
//     //     }
//     // }

    
// }
