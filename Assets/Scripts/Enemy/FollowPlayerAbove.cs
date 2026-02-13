
using UnityEngine;

public class FollowPlayerAbove : MonoBehaviour
{
    [Header("Timing")]
    public float attackToJianciDelay = 2f;
    public float jianciToAttackDelay = 3f;

    [Header("Attack")]
    public Vector2 offset = new Vector2(0f, 2f);

    [Header("Jianci")]
    public float jianciDuration = 1.5f;
    public string defaultAnimation = "Catwalk";

    private Transform playerTransform;
    private Animator anim;
    private Cat catScript;
    private Character character; // 用于回血

    private enum State
    {
        AttackPhase1_WaitBeforeMove,
        AttackPhase2_WaitAfterMove,
        JianciActive,
        DelayAfterAttack,
        DelayAfterJianci
    }

    private State currentState = State.DelayAfterJianci;
    private float timer = 0f;
    private Vector3 targetPosition;

    private float originalNormalSpeed = 0f;
    private float originalChaseSpeed = 0f;
    private float originalCurrentSpeed = 0f;

    private ParticleSystem greenGlowEffect;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        catScript = GetComponent<Cat>();
        character = GetComponent<Character>();

        if (catScript == null)
        {
            Debug.LogError("[FollowPlayerAbove] 未找到 Cat 脚本！", this);
        }
    }

    void Start()
    {
        TryFindPlayer();
        if (playerTransform == null)
        {
            Debug.LogWarning("[FollowPlayerAbove] 未立即找到 Player，将在 Update 中重试。", this);
        }

        CreateGreenGlowEffect();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            TryFindPlayer();
            return;
        }

        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.DelayAfterJianci:
                if (timer >= jianciToAttackDelay)
                {
                    if (anim != null) anim.Play("attack", -1, 0f);
                    targetPosition = playerTransform.position + new Vector3(offset.x, offset.y, 0f);
                    currentState = State.AttackPhase1_WaitBeforeMove;
                    timer = 0f;
                }
                break;

            case State.AttackPhase1_WaitBeforeMove:
                if (timer >= 0.8f)
                {
                    transform.position = targetPosition;

                    if (catScript != null)
                    {
                        originalNormalSpeed = catScript.NormalSpeed;
                        originalChaseSpeed = catScript.ChaseSpeed;
                        originalCurrentSpeed = catScript.CurrentSpeed;

                        catScript.NormalSpeed = 0f;
                        catScript.ChaseSpeed = 0f;
                        catScript.CurrentSpeed = 0f;
                    }

                    currentState = State.AttackPhase2_WaitAfterMove;
                    timer = 0f;
                }
                break;

            case State.AttackPhase2_WaitAfterMove:
                if (timer >= 0.8f)
                {
                    if (catScript != null)
                    {
                        catScript.NormalSpeed = originalNormalSpeed;
                        catScript.ChaseSpeed = originalChaseSpeed;
                        catScript.CurrentSpeed = originalCurrentSpeed;
                    }

                    currentState = State.DelayAfterAttack;
                    timer = 0f;
                }
                break;

            case State.DelayAfterAttack:
                if (timer >= attackToJianciDelay)
                {
                    if (anim != null) anim.Play("jianci", -1, 0f);

                    if (catScript != null)
                    {
                        originalNormalSpeed = catScript.NormalSpeed;
                        originalChaseSpeed = catScript.ChaseSpeed;
                        originalCurrentSpeed = catScript.CurrentSpeed;

                        catScript.NormalSpeed = 0f;
                        catScript.ChaseSpeed = 0f;
                        catScript.CurrentSpeed = 0f;
                    }

                    // ★★★ 回血逻辑：+10 HP，不超过 MaxHealth ★★★
                    if (character != null)
                    {
                        character.currentHealth = Mathf.Min(character.currentHealth + 10, character.maxHealth);
                    }

                    // 启动绿光特效
                    if (greenGlowEffect != null)
                    {
                        greenGlowEffect.transform.position = transform.position;
                        var emission = greenGlowEffect.emission;
                        emission.enabled = true;
                        greenGlowEffect.Play();
                    }

                    currentState = State.JianciActive;
                    timer = 0f;
                }
                break;

            case State.JianciActive:
                if (greenGlowEffect != null)
                {
                    greenGlowEffect.transform.position = transform.position;
                }

                if (timer >= jianciDuration)
                {
                    if (anim != null) anim.Play(defaultAnimation);

                    if (catScript != null)
                    {
                        catScript.NormalSpeed = originalNormalSpeed;
                        catScript.ChaseSpeed = originalChaseSpeed;
                        catScript.CurrentSpeed = originalCurrentSpeed;
                    }

                    if (greenGlowEffect != null)
                    {
                        greenGlowEffect.Stop();
                        var emission = greenGlowEffect.emission;
                        emission.enabled = false;
                    }

                    currentState = State.DelayAfterJianci;
                    timer = 0f;
                }
                break;
        }
    }

    // ★★★ 关键：补全缺失的方法 ★★★
    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void CreateGreenGlowEffect()
    {
        if (greenGlowEffect != null) return;

        GameObject effectObj = new GameObject("GreenGlowEffect");
        effectObj.transform.SetParent(transform);
        effectObj.transform.localPosition = Vector3.zero;

        greenGlowEffect = effectObj.AddComponent<ParticleSystem>();

        var main = greenGlowEffect.main;
        main.duration = 4f;
        main.loop = true;
        main.startLifetime = 1.8f;
        main.startSize = 0.8f;
        main.startSpeed = 0.4f;
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var renderer = greenGlowEffect.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
            renderer = greenGlowEffect.gameObject.AddComponent<ParticleSystemRenderer>();

        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));

        var colOverLifetime = greenGlowEffect.colorOverLifetime;
        colOverLifetime.enabled = true;
        colOverLifetime.color = new ParticleSystem.MinMaxGradient(new Color(0f, 1f, 0.6f, 0.7f));

        var emission = greenGlowEffect.emission;
        emission.rateOverTime = 12f;

        var shape = greenGlowEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.7f;

        emission.enabled = false;
        greenGlowEffect.Stop();

        DontDestroyOnLoad(effectObj);
    }

    private void OnDisable()
    {
        if (greenGlowEffect != null)
        {
            greenGlowEffect.Stop();
            var emission = greenGlowEffect.emission;
            emission.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (greenGlowEffect != null && greenGlowEffect.gameObject != null)
        {
            Destroy(greenGlowEffect.gameObject);
        }
    }
}