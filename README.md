# Segunda Versión - Entornos y Simulación

## Descripción del Proyecto

Este proyecto consiste en un entorno 3D interactivo desarrollado en Unity, ambientado en una temática inspirada en el universo Pokémon. El jugador controla a Jigglypuff, quien debe recorrer diferentes zonas del mapa mientras interactúa con múltiples simulaciones físicas integradas al entorno.

El objetivo del proyecto es demostrar la implementación de simulaciones basadas en principios matemáticos y físicos reales, sin depender del sistema Rigidbody de Unity. Todas las interacciones físicas fueron desarrolladas mediante integración numérica, aplicación de fuerzas, impulsos y detección de colisiones personalizadas.

---

## Simulaciones Implementadas

### 1. Magnetismo

La bolita es atraída o repelida por campos magnéticos mediante una fuerza que disminuye con el cuadrado de la distancia.

### 2. Viento

Una fuerza direccional empuja al jugador. Puede funcionar de forma constante u oscilante utilizando funciones sinusoidales.

### 3. Salto

Se aplica un impulso vertical instantáneo que permite superar obstáculos y desplazarse entre plataformas.

### 4. Boost

Se aplica un impulso horizontal que incrementa temporalmente la velocidad del jugador, similar a los aceleradores presentes en juegos de carreras.

### 5. Superficies

Diferentes zonas modifican el coeficiente de fricción del terreno, simulando materiales con distintos comportamientos físicos.

---

## Características Principales

* Entorno 3D navegable.
* Control mediante teclado.
* Compatibilidad con gamepad.
* Sistema físico propio sin Rigidbody.
* Simulaciones integradas al entorno.
* Parámetros modificables en tiempo real mediante interfaz gráfica.
* HUD informativo.
* Música ambiental.
* Sistema de colisiones personalizado.

---

## Controles

### Teclado

* W → Avanzar
* S → Retroceder
* A → Mover a la izquierda
* D → Mover a la derecha

También es posible utilizar las teclas de dirección.

### Gamepad

* Stick izquierdo → Movimiento del personaje

---

## Descarga y Ejecución

1. Acceder al repositorio del proyecto.
2. Abrir la sección **Releases**.
3. Descargar la versión:

**Segunda Versión - Entornos y Simulación**

4. Extraer el archivo descargado.
5. Ejecutar el archivo principal del juego (.exe).
6. Esperar la carga inicial del entorno.
7. Recorrer el mapa e interactuar con las diferentes simulaciones físicas.

---

## Tecnologías Utilizadas

* Unity
* C#
* TextMeshPro
* Sistema de físicas personalizado

---

## Autores

* Sara Sofia Moreno Castro
* Angy Juliana Farasica Eljaiek

---

## Información Académica

Proyecto desarrollado para la asignatura Entornos y Simulación / Electiva de Profundización 1
