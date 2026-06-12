# Parte 1: Investigación Teórica
## Estructuras de Datos Avanzadas y APIs con ASP.NET Core

---

## 1. Estructuras de Datos Eficientes

### Árboles Binarios de Búsqueda (ABB)

Un **Árbol Binario de Búsqueda (ABB)** es una estructura de datos jerárquica donde cada nodo sigue una regla de ordenamiento estricta:

- **Hijo izquierdo:** Contiene un valor **menor** que el nodo padre.
- **Hijo derecho:** Contiene un valor **mayor** que el nodo padre.

Esta propiedad se aplica de forma recursiva en todos los subárboles del árbol.

**Ejemplo visual:**

```
        10
       /  \
      5    15
     / \
    3   7
```

En este árbol: `3 < 5 < 7 < 10 < 15`. Cada nodo cumple la regla de ordenamiento.

---

**Principal desventaja: Degeneración en lista vinculada**

Cuando los datos se insertan en **orden secuencial** (ascendente o descendente), el árbol pierde su estructura jerárquica y se convierte en una lista vinculada.

Por ejemplo, si insertamos `5 → 10 → 15 → 20 → 25`:

```
5
 \
  10
   \
    15
     \
      20
       \
        25
```

En este caso degenerado, la complejidad de búsqueda pasa de **O(log n)** en el caso ideal a **O(n)** en el peor caso, ya que hay que recorrer todos los nodos linealmente. Esto elimina completamente la ventaja del árbol sobre una lista simple.

---

### Árboles AVL

Un **Árbol AVL** (Adelson-Velsky y Landis, 1962) es un Árbol Binario de Búsqueda **auto-balanceado**. Esto significa que, después de cada inserción o eliminación, el árbol se reorganiza automáticamente para mantener su altura mínima y evitar la degeneración.

**Factor de Balanceo**

El factor de balanceo de cada nodo se calcula como:

$$Factor = Altura_{Izquierda} - Altura_{Derecha}$$

- Factor en **{-1, 0, +1}** → nodo balanceado ✅
- Factor **+2 o -2** → árbol desbalanceado, se aplica una **rotación** para corregirlo ❌

**Tipos de rotaciones para rebalancear:**

| Caso | Rotación aplicada |
|------|-------------------|
| Desbalanceo a la izquierda | Rotación simple derecha |
| Desbalanceo a la derecha | Rotación simple izquierda |
| Desbalanceo izquierda-derecha | Rotación doble izq-der |
| Desbalanceo derecha-izquierda | Rotación doble der-izq |

**¿Por qué la complejidad se mantiene siempre en O(log n)?**

Gracias al balanceo automático, la altura del árbol AVL **nunca supera** `1.44 × log₂(n)`. Esto garantiza que sin importar el orden de inserción, el árbol permanece "bajo y ancho" en lugar de volverse una lista larga. Al mantener la altura acotada logarítmicamente, todas las operaciones encuentran o modifican cualquier elemento en `O(log n)` pasos incluso en el peor caso.

| Operación   | ABB peor caso | AVL siempre |
|-------------|---------------|-------------|
| Búsqueda    | O(n)          | O(log n)    |
| Inserción   | O(n)          | O(log n)    |
| Eliminación | O(n)          | O(log n)    |

---

## 2. Fundamentos de Web APIs

### ¿Qué es una API y cómo funciona el modelo Cliente-Servidor?

Una **API (Application Programming Interface)** es un conjunto de reglas que permite que dos sistemas de software se comuniquen entre sí. En el contexto web, una **Web API** expone datos y funcionalidades a través del protocolo HTTP para que aplicaciones externas puedan consumirlos.

**Modelo Cliente-Servidor**

El modelo divide los roles en dos partes claramente separadas:

- **Cliente:** Inicia la comunicación. Puede ser un navegador, una app móvil, o una herramienta como Postman. Formula una **petición (Request)** y espera una respuesta.
- **Servidor:** Escucha peticiones entrantes, las procesa y devuelve una **respuesta (Response)**.

**Flujo de una petición HTTP:**

```
CLIENTE                                         SERVIDOR
  |                                                 |
  |  1. REQUEST                                     |
  |  GET /api/nodos HTTP/1.1                        |
  |  Host: localhost:5000                           |
  |  Content-Type: application/json                 |
  | ──────────────────────────────────────────────> |
  |                                                 |
  |                               2. Procesa la    |
  |                                  solicitud     |
  |                                                 |
  |  3. RESPONSE                                    |
  |  HTTP/1.1 200 OK                                |
  |  Body: [{"id":10,"valor":"Raíz"}]              |
  | <────────────────────────────────────────────── |
```

Una **Request** contiene:
- Método HTTP (GET, POST, etc.)
- URL del recurso
- Headers con metadatos
- Body opcional con datos

Una **Response** contiene:
- Código de estado HTTP (200, 201, 400, 404...)
- Headers de respuesta
- Body con los datos devueltos (generalmente JSON)

---

### Verbos HTTP: GET y POST

#### GET — Recuperación de recursos

El verbo **GET** se utiliza para **leer o recuperar** un recurso existente del servidor. No debe modificar ningún dato en el servidor; su único propósito es consultar.

- **Uso correcto:** Obtener una lista de elementos, buscar un registro por ID, consultar el estado de un recurso.
- **Respuesta exitosa:** `200 OK` con los datos en el body.
- **¿Lleva body?** No, la petición GET no incluye body.

**Idempotencia:** GET **sí es idempotente**. Esto significa que ejecutar la misma petición GET una vez o cien veces produce exactamente el mismo resultado y el estado del servidor **no cambia**. Es una operación de solo lectura.

```
GET /api/nodos  →  200 OK  (misma respuesta siempre)
GET /api/nodos  →  200 OK  (el servidor no se modificó)
GET /api/nodos  →  200 OK  (idem)
```

---

#### POST — Creación de nuevos recursos

El verbo **POST** se utiliza para **crear un nuevo recurso** en el servidor. El cliente envía los datos del nuevo recurso en el body de la petición.

- **Uso correcto:** Registrar un nuevo usuario, insertar un elemento, enviar un formulario.
- **Respuesta exitosa:** `201 Created` con el recurso recién creado en el body.
- **¿Lleva body?** Sí, contiene los datos del nuevo recurso en formato JSON.

**Idempotencia:** POST **no es idempotente**. Cada vez que se envía la misma petición POST, se crea un nuevo recurso en el servidor. El estado del servidor **cambia** con cada llamada.

```
POST /api/nodos { "id": 15, "valor": "Nodo" }  →  201 Created (1 nodo creado)
POST /api/nodos { "id": 15, "valor": "Nodo" }  →  201 Created (otro nodo creado)
POST /api/nodos { "id": 15, "valor": "Nodo" }  →  201 Created (otro más)
```

**Comparativa final:**

| Característica    | GET                        | POST                        |
|-------------------|----------------------------|-----------------------------|
| Propósito         | Leer / Recuperar           | Crear un nuevo recurso      |
| Modifica el estado| No                         | Sí                          |
| Lleva body        | No                         | Sí (datos del nuevo recurso)|
| Respuesta exitosa | 200 OK                     | 201 Created                 |
| Idempotente       |  Sí                      |  No                       |




**Repositorio Git**
https://github.com/202505114/Actividad-11-06.git

