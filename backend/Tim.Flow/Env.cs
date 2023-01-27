// <copyright file="Env.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Flow
{
    using System;

    public static class Env
    {
        private static readonly object s_envLock = new();
        private static bool s_isEnvSet;

        public static RuntimeEnvironment CurrentEnvironment
        {
            get;
            private set;
        }

        public static void SetEnvironment(RuntimeEnvironment environment)
        {
            lock (s_envLock)
            {
                if (s_isEnvSet)
                {
                    throw new InvalidOperationException("Cannot set the Runtime Environment twice");
                }

                CurrentEnvironment = environment;
                s_isEnvSet = true;
            }
        }

        public static RuntimeEnvironment GetRuntimeEnvironmentFromEnvVariable(string envVar, RuntimeEnvironment defaultVal)
        {
            return Enum.TryParse(Environment.GetEnvironmentVariable(envVar), true, out RuntimeEnvironment parsedEnv) ? parsedEnv : defaultVal;
        }

        public static T Current<T>(T ppeValue, T prodValue)
        {
            return CurrentEnvironment == RuntimeEnvironment.PROD ? prodValue : ppeValue;
        }

        public static T Current<T>(Func<T> ppeOperation, Func<T> prodOperation)
        {
            return CurrentEnvironment == RuntimeEnvironment.PROD ? prodOperation() : ppeOperation();
        }

        public static void Current(Action ppeAction, Action prodAction)
        {
            if (CurrentEnvironment == RuntimeEnvironment.PROD)
            {
                prodAction();
                return;
            }

            ppeAction();
        }
    }

    public enum RuntimeEnvironment
    {
        PPE,
        PROD
    }
}
