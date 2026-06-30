using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Bestuurt de speler (ruimteschip): beweging met de muis, score bijhouden,
// en wat er gebeurt bij een botsing met een obstakel.
public class PlayerController : MonoBehaviour
{
    // --- Instelbaar in de Unity Inspector ---
    public GameObject explosionEffectPrefab; // Explosie die spawnt bij dood
    public GameObject flame;                  // Vlam-object achter het schip (aan tijdens stuwen)
    public UIDocument uiDocument;             // Het UI-scherm (score, high score, restart-knop)
    public AudioClip explosionSound;          // Geluid bij dood/explosie
    public AudioClip thrustSound;             // Motorgeluid tijdens stuwen (loopt)
    public float thrustForce = 1f;            // Hoe hard het schip wordt geduwd
    public float maxSpeed = 5f;               // Maximale snelheid van het schip
    public float scoreMultiplier = 10f;       // Score per seconde overleefd

    // --- Interne variabelen (niet zichtbaar in Inspector) ---
    private float _elapsedTime = 0f;          // Tijd overleefd in seconden
    private float _score = 0f;                // Huidige score
    private Label _scoreLabel;                // UI-tekst voor de score
    private Label _highScoreLabel;            // UI-tekst voor de high score
    private Button _restartButton;            // Knop om opnieuw te starten
    private Rigidbody2D _rb;                  // Physics-body van het schip
    private AudioSource _thrustSource;        // Speelt het loopende motorgeluid

    // Start: wordt 1x aangeroepen bij het opstarten van het object.
    void Start()
    {
        // Pak de physics-body van dit object
        _rb = GetComponent<Rigidbody2D>();

        // Maak een AudioSource aan voor het loopende motorgeluid
        _thrustSource = gameObject.AddComponent<AudioSource>();
        _thrustSource.clip = thrustSound;
        _thrustSource.loop = true;

        // Zoek de UI-onderdelen op via hun naam in het UI-document
        _scoreLabel = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        _highScoreLabel = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        _restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        // Koppel de restart-knop aan de herstart-functie
        _restartButton.clicked += ReloadScene;
    }

    // Update: wordt elke frame aangeroepen (bv. 60x per seconde).
    void Update()
    {
        UpdateScore();
        MovePlayer();
    }

    // Wordt aangeroepen zodra het schip iets raakt (een obstakel).
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Speel explosiegeluid los van dit object (overleeft de Destroy hieronder)
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        Destroy(gameObject);                                                 // Vernietig het schip
        Instantiate(explosionEffectPrefab, transform.position, transform.rotation); // Spawn explosie
        _restartButton.style.display = DisplayStyle.Flex;                    // Toon de restart-knop
        ShowHighScore();                                                     // Werk high score bij + toon
    }

    // Vergelijkt de huidige score met de opgeslagen high score en toont die.
    private void ShowHighScore()
    {
        // PlayerPrefs = lokale opslag op het apparaat (blijft bewaard na afsluiten)
        if (_score > PlayerPrefs.GetFloat("highScore"))
        {
            PlayerPrefs.SetFloat("highScore", _score); // Nieuwe high score opslaan
        }

        _highScoreLabel.text = "High Score: " + PlayerPrefs.GetFloat("highScore");
        _highScoreLabel.style.display = DisplayStyle.Flex; // Maak het label zichtbaar
    }

    // Telt de overleefde tijd op en berekent de score.
    private void UpdateScore()
    {
        _elapsedTime += Time.deltaTime;                          // deltaTime = tijd sinds vorige frame
        _score = Mathf.FloorToInt(_elapsedTime * scoreMultiplier); // Afronden naar heel getal
        _scoreLabel.text = "Score: " + _score;
    }

    // Beweegt het schip richting de muis zolang de linkermuisknop ingedrukt is.
    private void MovePlayer()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            // Bereken de richting van het schip naar de muis-positie
            var mousePos = Camera.main!.ScreenToWorldPoint(Mouse.current.position.value);
            var direction = (mousePos - transform.position).normalized; // genormaliseerd = lengte 1
            direction.z = 0; // 2D-game: z-as negeren

            // Draai het schip naar de muis en geef stuwkracht
            transform.up = direction;
            _rb.AddForce(direction * thrustForce);

            // Begrens de snelheid op maxSpeed
            if (_rb.linearVelocity.magnitude > maxSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
            }
        }

        // Vlam aan op het moment van indrukken, uit bij loslaten
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            flame.SetActive(true);
            if (thrustSound != null)
                _thrustSource.Play();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            flame.SetActive(false);
            _thrustSource.Stop();
        }
    }

    // Herlaadt de huidige scene = het spel opnieuw starten.
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
