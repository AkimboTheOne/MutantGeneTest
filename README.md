# MutantGeneTest

Es un laboratorio que hace parte de los ejercicios técnicos para un Assessment de selección de Líderes Técnicos / Líderes de Proyecto.

## Instrucciones de Uso

La aplicación expone un REST API.

URL Documentación Open API: **{baseurl}/swagger**

API End-Point: **{baseurl}/api/**

1- Método de validación:

POST | **mutant | {baseurl}/api/mutant**

Cuerpo (BODY):

**{**

**&quot;dna&quot;:[&quot;ATGCGA&quot;, &quot;CAGTGC&quot;, &quot;TTATTT&quot;, &quot;AGACGG&quot;, &quot;GCGTCA&quot;, &quot;TCACTG&quot;]**

**}**

Respuestas:

[ProducesResponseType(typeof(DnaClass), StatusCodes.Status200OK)]

[ProducesResponseType(typeof(ErrorEnvelopeClass), StatusCodes.Status400BadRequest)]

2- Método de estadísticas:

GET | **mutant | {baseurl}/api/mutant/stats**

Respuestas:

[ProducesResponseType(typeof(DnaStatsClass), StatusCodes.Status200OK)]

[ProducesResponseType(typeof(ErrorEnvelopeClass), StatusCodes.Status400BadRequest)]

## Puntos de atención

- Para optimizar la elegancia en el desarrollo, mantenimiento y lectura del código, se utilizó un estilo/patrón de codificación &quot;Guardián&quot; que interrumpe eficientemente la ejecución de código que requieren validaciones lógicas o por generación de excepciones.
- Estudiar la cadena dna humano es más demandante computacionalmente ya que debe recorrer toda la matriz.
- Los algoritmos de identificación de cadenas se podrían optimizar en otras iteraciones. Y se podrían implementar directamente en el laboratorio.

## Limitaciones

Debido a las capacidades limitadas del ambiente de desarrollo dispuesto, se presentarán las siguientes restricciones controladas para evitar el desbordamiento:

- No requiere de esquemas de autenticación y autorización.
- Tiempo máximo de ejecución computacional diario de hilos es de 60 minutos. Valor no controlado por el desarrollo.
- La capacidad funcional solo acepta matrices cuadráticas NxN de hasta un máximo de 100. Valor estático configurable.
- El sistema de persistencia (Nivel 3) soporta un almacenamiento de máximo 10.000 objetos. Valor estático configurable.
- Ante la inactividad mayor de 5 minutos y dentro del tiempo máximo de ejecución, el controlador reiniciará y se perderán los datos de la persistencia. Valor no controlado por el desarrollo.
