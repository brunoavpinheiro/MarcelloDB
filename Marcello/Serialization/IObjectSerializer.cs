﻿using System;

namespace Marcello.Serialization
{
    public interface IObjectSerializer<T>
    {
        byte[] Serialize(T obj);

        T Deserialize(byte[] bytes);
    }
}

