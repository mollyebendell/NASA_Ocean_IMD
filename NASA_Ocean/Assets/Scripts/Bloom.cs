using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bloom : MonoBehaviour
{
    static float simHeight = 1.1f;
    static float cScale = 10;
    static float simWidth = 100;

    static int U_FIELD = 0;
    static int V_FIELD = 1;
    static int S_FIELD = 2;

    float cnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    class Fluid
    {
        private float density;
        private int numX, numY, numCells;
        private float h;
        private float[] u, v, newU, newV, p, s, m, newM;

        public Fluid(float density, int numX, int numY, float h)
        {
            this.density = density;
            this.numX = numX + 2;
            this.numY = numY + 2;
            this.numCells = this.numX * this.numY;
            this.h = h;
            this.u = new float[this.numCells];
            this.v = new float[this.numCells];
            this.newU = new float[this.numCells];
            this.newV = new float[this.numCells];
            this.p = new float[this.numCells];
            this.s = new float[this.numCells];
            this.m = new float[this.numCells];
            this.newM = new float[this.numCells];
            Array.Fill(this.m, 1.0f);
            int num = numX * numY;
        }

        public void Integrate(float dt, float gravity)
        {
            int n = this.numY;
            for (int i = 1; i < this.numX; i++)
            {
                for (int j = 1; j < this.numY - 1; j++)
                {
                    if (this.s[i * n + j] != 0.0f && this.s[i * n + j - 1] != 0.0f)
                        this.v[i * n + j] += gravity * dt;
                }
            }
        }

        public void SolveIncompressibility(int numIters, float dt)
        {
            int n = this.numY;
            float cp = this.density * this.h / dt;

            for (int iter = 0; iter < numIters; iter++)
            {
                for (int i = 1; i < this.numX - 1; i++)
                {
                    for (int j = 1; j < this.numY - 1; j++)
                    {
                        if (this.s[i * n + j] == 0.0f)
                            continue;

                        float s = this.s[i * n + j];
                        float sx0 = this.s[(i - 1) * n + j];
                        float sx1 = this.s[(i + 1) * n + j];
                        float sy0 = this.s[i * n + j - 1];
                        float sy1 = this.s[i * n + j + 1];
                        s = sx0 + sx1 + sy0 + sy1;
                        if (s == 0.0f)
                            continue;

                        float div = this.u[(i + 1) * n + j] - this.u[i * n + j] +
                                    this.v[i * n + j + 1] - this.v[i * n + j];

                        float pressure = -div / s;
                        pressure *= 1.9f; // Assuming scene.overRelaxation = 1.9
                        this.p[i * n + j] += cp * pressure;

                        this.u[i * n + j] -= sx0 * pressure;
                        this.u[(i + 1) * n + j] += sx1 * pressure;
                        this.v[i * n + j] -= sy0 * pressure;
                        this.v[i * n + j + 1] += sy1 * pressure;
                    }
                }
            }
        }

        public void Extrapolate()
        {
            int n = this.numY;
            for (int i = 0; i < this.numX; i++)
            {
                this.u[i * n + 0] = this.u[i * n + 1];
                this.u[i * n + this.numY - 1] = this.u[i * n + this.numY - 2];
            }
            for (int j = 0; j < this.numY; j++)
            {
                this.v[0 * n + j] = this.v[1 * n + j];
                this.v[(this.numX - 1) * n + j] = this.v[(this.numX - 2) * n + j];
            }
        }

        public float SampleField(float x, float y, int field)
        {
            int n = this.numY;
            float h1 = 1.0f / this.h;
            float h2 = 0.5f * this.h;

            x = Math.Max(Math.Min(x, this.numX * this.h), this.h);
            y = Math.Max(Math.Min(y, this.numY * this.h), this.h);

            float dx = 0.0f;
            float dy = 0.0f;

            float[] f;
            switch (field)
            {
                case 0: f = this.u; dy = h2; break;
                case 1: f = this.v; dx = h2; break;
                case 2: f = this.m; dx = h2; dy = h2; break;
                default: f = this.u; break;
            }

            int x0 = Math.Min((int)((x - dx) * h1), this.numX - 1);
            float tx = ((x - dx) - x0 * this.h) * h1;
            int x1 = Math.Min(x0 + 1, this.numX - 1);

            int y0 = Math.Min((int)((y - dy) * h1), this.numY - 1);
            float ty = ((y - dy) - y0 * this.h) * h1;
            int y1 = Math.Min(y0 + 1, this.numY - 1);

            float sx = 1.0f - tx;
            float sy = 1.0f - ty;

            float val = sx * sy * f[x0 * n + y0] +
                        tx * sy * f[x1 * n + y0] +
                        tx * ty * f[x1 * n + y1] +
                        sx * ty * f[x0 * n + y1];

            return val;
        }

        public float AvgU(int i, int j)
        {
            int n = this.numY;
            float u = (this.u[i * n + j - 1] + this.u[i * n + j] +
                       this.u[(i + 1) * n + j - 1] + this.u[(i + 1) * n + j]) * 0.25f;
            return u;
        }

        public float AvgV(int i, int j)
        {
            int n = this.numY;
            float v = (this.v[(i - 1) * n + j] + this.v[i * n + j] +
                       this.v[(i - 1) * n + j + 1] + this.v[i * n + j + 1]) * 0.25f;
            return v;
        }


        public void AdvectVel(float dt)
        {
            Array.Copy(u, newU, u.Length);
            Array.Copy(v, newV, v.Length);

            int n = numY;
            float h = this.h;
            float h2 = 0.5f * h;

            for (int i = 1; i < numX; i++)
            {
                for (int j = 1; j < numY; j++)
                {
                    // u component
                    if (s[i * n + j] != 0.0f && s[(i - 1) * n + j] != 0.0f && j < numY - 1)
                    {
                        float x = i * h;
                        float y = j * h + h2;
                        float uu = u[i * n + j];
                        float vv = AvgV(i, j);
                        x = x - dt * uu;
                        y = y - dt * vv;
                        uu = SampleField(x, y, U_FIELD);
                        newU[i * n + j] = uu;
                    }
                    // v component
                    if (s[i * n + j] != 0.0f && s[i * n + j - 1] != 0.0f && i < numX - 1)
                    {
                        float x = i * h + h2;
                        float y = j * h;
                        float uu = AvgU(i, j);
                        float vv = v[i * n + j];
                        x = x - dt * uu;
                        y = y - dt * vv;
                        vv = SampleField(x, y, V_FIELD);
                        newV[i * n + j] = vv;
                    }
                }
            }

            Array.Copy(newU, u, newU.Length);
            Array.Copy(newV, v, newV.Length);
        }

        public void AdvectSmoke(float dt)
        {
            Array.Copy(m, newM, m.Length);

            int n = numY;
            float h = this.h;
            float h2 = 0.5f * h;

            for (int i = 1; i < numX - 1; i++)
            {
                for (int j = 1; j < numY - 1; j++)
                {
                    if (s[i * n + j] != 0.0f)
                    {
                        float uu = (u[i * n + j] + u[(i + 1) * n + j]) * 0.5f;
                        float vv = (v[i * n + j] + v[i * n + j + 1]) * 0.5f;
                        float x = i * h + h2 - dt * uu;
                        float y = j * h + h2 - dt * vv;

                        newM[i * n + j] = SampleField(x, y, S_FIELD);
                    }
                }
            }

            Array.Copy(newM, m, newM.Length);
        }

    }


}
