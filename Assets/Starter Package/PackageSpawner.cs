/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PackageSpawner : MonoBehaviour
{
    public DrivingSurfaceManager DrivingSurfaceManager;
    public PackageBehaviour Package;
    public GameObject PackagePrefab;

    private bool isPackageInitialized = false;
    private Vector3 oldPosition;

    public static Vector3 RandomInTriangle(Vector3 v1, Vector3 v2)
    {
        float u = Random.Range(0.0f, 1.0f);
        float v = Random.Range(0.0f, 1.0f);
        if (v + u > 1)
        {
            v = 1 - v;
            u = 1 - u;
        }

        return (v1 * u) + (v2 * v);
    }

    public static Vector3 FindRandomLocation(ARPlane plane)
    {
        // Select random triangle in Mesh
        var mesh = plane.GetComponent<ARPlaneMeshVisualizer>().mesh;
        var triangles = mesh.triangles;
        var triangle = triangles[(int)Random.Range(0, triangles.Length - 1)] / 3 * 3;
        var vertices = mesh.vertices;
        var randomInTriangle = RandomInTriangle(vertices[triangle], vertices[triangle + 1]);
        var randomPoint = plane.transform.TransformPoint(randomInTriangle);

        return randomPoint;
    }

    private Vector3 bezier(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (1 - t) * (1 - t) * p1 + 2 * t * (1 - t) * p2 + t * t * p3;
    }

    IEnumerator Relocate(ARPlane plane, GameObject package)
    {
        package.GetComponent<BoxCollider>().isTrigger = false;
        var newPosition = FindRandomLocation(plane);
        var tempPoint = new Vector3(
            (oldPosition.x + newPosition.x) / 2, 
            (oldPosition.y + newPosition.y) / 2 + 2f, 
            (oldPosition.z + newPosition.z) / 2);
        for (float t = 0f; t <= 1f; t += 0.02f)
        {
            package.transform.position = bezier(t, oldPosition, tempPoint, newPosition);
            //package.transform.Rotate(new Vector3(0f, 0f, 5f));
            yield return null;
        }
        //package.transform.position = newPosition;
        oldPosition = newPosition;
        package.GetComponent<BoxCollider>().isTrigger = true;
    }

    public void SpawnPackage(ARPlane plane)
    {
        var packageClone = GameObject.Instantiate(PackagePrefab);
        Package = packageClone.GetComponent<PackageBehaviour>();
        if (!isPackageInitialized)
        {
            oldPosition = FindRandomLocation(plane);
            packageClone.transform.position = oldPosition;
        }
        else
        {
            StartCoroutine(Relocate(plane, packageClone));
        }
        //packageClone.transform.localScale *= Random.Range(0.5f, 2.0f);

        //Package = packageClone.GetComponent<PackageBehaviour>();
    }

    private void Update()
    {
        var lockedPlane = DrivingSurfaceManager.LockedPlane;
        if (lockedPlane != null)
        {
            if (Package == null)
            {
                SpawnPackage(lockedPlane);
                isPackageInitialized = true;
            }

            //var packagePosition = Package.gameObject.transform.position;
            //packagePosition.Set(packagePosition.x, lockedPlane.center.y, packagePosition.z);
        }
    }
}
