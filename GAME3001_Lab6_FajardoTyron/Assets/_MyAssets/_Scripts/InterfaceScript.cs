using UnityEngine;

public class InterfaceScript : MonoBehaviour
{
    public interface IEMPable
    {
        void ApplyEMP(float duration);
    }

    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}
