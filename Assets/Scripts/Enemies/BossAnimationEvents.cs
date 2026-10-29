using UnityEngine;

// Nos chefes, o Animator fica num objeto filho (o visual do personagem).
// Os Animation Events chegam aqui e são repassados para o script do chefe no objeto pai.
public class BossAnimationEvents : MonoBehaviour
{
    public void AttackHit() => Forward(nameof(AttackHit));
    public void ShootBeam() => Forward(nameof(ShootBeam));
    public void CastSpell() => Forward(nameof(CastSpell));

    private void Forward(string method) => transform.parent.SendMessageUpwards(method, SendMessageOptions.DontRequireReceiver);
}
