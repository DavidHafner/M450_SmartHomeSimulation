using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests.IsolationTests
{
    // --- Fake classes for testing ---

    public class FakeZimmer : Zimmer
    {
        public bool VerarbeiteWetterdatenCalled { get; private set; }
        public Wetterdaten? LetzteWetterdaten { get; private set; }

        public FakeZimmer(string name) : base(name) { }

        public override void VerarbeiteWetterdaten(Wetterdaten wetterdaten)
        {
            VerarbeiteWetterdatenCalled = true;
            LetzteWetterdaten = wetterdaten;
        }
    }

    public class FakeZimmerMitAktor : ZimmerMitAktor
    {
        public FakeZimmerMitAktor(Zimmer zimmer) : base(zimmer) { }
    }

    // --- Unit tests ---

    [TestClass]
    public class ZimmerMitAktorTests
    {
        [TestMethod]
        public void Konstruktor_ShouldStoreZimmerReference()
        {
            // Arrange
            var innerZimmer = new FakeZimmer("Wohnzimmer");

            // Act
            var aktorZimmer = new FakeZimmerMitAktor(innerZimmer);

            // Assert
            Assert.AreSame(innerZimmer, aktorZimmer.GetType().GetProperty("Zimmer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(aktorZimmer));
        }

        [TestMethod]
        public void Temperaturvorgabe_ShouldBeForwardedToInnerZimmer()
        {
            // Arrange
            var inner = new FakeZimmer("Test");
            var aktorZimmer = new FakeZimmerMitAktor(inner);

            // Act
            aktorZimmer.Temperaturvorgabe = 22.5;

            // Assert
            Assert.AreEqual(22.5, inner.Temperaturvorgabe);
        }

        [TestMethod]
        public void PersonenImZimmer_ShouldBeForwardedToInnerZimmer()
        {
            // Arrange
            var inner = new FakeZimmer("Test");
            var aktorZimmer = new FakeZimmerMitAktor(inner);

            // Act
            aktorZimmer.PersonenImZimmer = true;

            // Assert
            Assert.IsTrue(inner.PersonenImZimmer);
        }

        [TestMethod]
        public void VerarbeiteWetterdaten_ShouldDelegateToInnerZimmer()
        {
            // Arrange
            var inner = new FakeZimmer("Test");
            var aktorZimmer = new FakeZimmerMitAktor(inner);
            var wetterdaten = new Wetterdaten { Aussentemperatur = 20, Windgeschwindigkeit = 10, Regen = false };

            // Act
            aktorZimmer.VerarbeiteWetterdaten(wetterdaten);

            // Assert
            Assert.IsTrue(inner.VerarbeiteWetterdatenCalled);
            Assert.AreEqual(wetterdaten, inner.LetzteWetterdaten);
        }

        [TestMethod]
        public void GetZimmerMitAktor_ShouldReturnMatchingType()
        {
            // Arrange
            var inner = new FakeZimmer("Test");
            var aktorZimmer = new FakeZimmerMitAktor(inner);

            // Act
            var result = aktorZimmer.GetZimmerMitAktor<FakeZimmerMitAktor>();

            // Assert
            Assert.AreSame(aktorZimmer, result);
        }

        [TestMethod]
        public void GetZimmerMitAktor_ShouldRecurseThroughNestedZimmerMitAktor()
        {
            // Arrange
            var baseZimmer = new FakeZimmer("Test");
            var middleLayer = new FakeZimmerMitAktor(baseZimmer);
            var outerLayer = new FakeZimmerMitAktor(middleLayer);

            // Act
            var result = outerLayer.GetZimmerMitAktor<FakeZimmerMitAktor>();

            // Assert
            Assert.AreSame(outerLayer, result); // the first matching type in the chain
        }

        [TestMethod]
        public void GetZimmerMitAktor_ShouldReturnNullIfTypeNotFound()
        {
            // Arrange
            var inner = new FakeZimmer("Test");
            var aktorZimmer = new FakeZimmerMitAktor(inner);

            // Act
            var result = aktorZimmer.GetZimmerMitAktor<FakeZimmer>();

            // Assert
            Assert.IsNull(result);
        }
    }
}