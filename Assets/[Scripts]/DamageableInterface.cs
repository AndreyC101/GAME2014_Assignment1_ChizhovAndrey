using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DamageableInterface.cs - Andrey Chizhov - 101255069
/// Implemented by any object that can be destroyed by combat
/// </summary>
public interface IDamageable
{
    bool TakeDamage(int damage);
    void HandleDestruction();
    void ClearFromField();
}
