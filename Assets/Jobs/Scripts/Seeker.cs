﻿using System;
using UnityEngine;

namespace LearnJobs
{
    public class Seeker : MonoBehaviour
    {
        public Vector3 Direction;

        public void Update()
        {
            transform.localPosition += Direction * Time.deltaTime;
        }
    }
}