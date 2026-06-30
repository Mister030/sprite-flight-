using UnityEngine;
using Random = UnityEngine.Random;

// Bestuurt een obstakel: krijgt bij het spawnen een willekeurige grootte,
// richting, snelheid en draaiing. Spawnt een bounce-effect bij een botsing.
public class Obstacle : MonoBehaviour
{
    // --- Instelbaar in de Unity Inspector ---
    public GameObject bounceEffectPrefab; // Effect dat spawnt op het botspunt
    public AudioClip bounceSound;         // Geluid bij een botsing
    public float minSize = 0.5f;          // Kleinste mogelijke grootte
    public float maxSize = 2.0f;          // Grootste mogelijke grootte
    public float minSpeed = 50f;          // Laagste snelheid
    public float maxSpeed = 200f;         // Hoogste snelheid
    public float maxSpinSpeed = 10f;      // Maximale draaisnelheid

    private Rigidbody2D _rb;              // Physics-body van het obstakel

    // Start: wordt 1x aangeroepen bij het spawnen van het obstakel.
    private void Start()
    {
        // Willekeurige grootte kiezen en toepassen
        var randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        // Snelheid: kleiner obstakel = sneller (snelheid gedeeld door grootte)
        var randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        var randomDirection = Random.insideUnitCircle; // Willekeurige richting

        // Geef het obstakel een zet in die richting
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(randomDirection * randomSpeed);

        // Geef het obstakel een willekeurige draaiing (links of rechts)
        var randomSpinSpeed = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        _rb.AddTorque(randomSpinSpeed);
    }

    // Wordt aangeroepen zodra het obstakel iets raakt.
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Pak het exacte punt waar de botsing plaatsvond
        var contactPoint = other.GetContact(0).point;

        // Speel botsgeluid op het botspunt
        if (bounceSound != null)
            AudioSource.PlayClipAtPoint(bounceSound, contactPoint);

        // Spawn een bounce-effect op dat punt
        var bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);

        // Maak het effect groter naarmate de botsing harder was
        bounceEffect.transform.localScale *= other.relativeVelocity.magnitude;

        // Verwijder het effect na 1 seconde
        Destroy(bounceEffect, 1f);
    }
}
