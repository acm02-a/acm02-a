# Comisiones de vendedores — Manto

**Estado:** borrador v3 · Fase 1 (solo Arian) · Fecha: 2026-08-13
**Aplica a:** vendedores externos que traen clientes a Manto.

Este documento define cómo se paga a un vendedor. Las secciones 1 a 5 son las
que se comparten con el vendedor. La sección 6 es **interna** — no se comparte.

---

## 1. El modelo en una línea

El vendedor cobra **40 % del pago inicial (setup) del cliente, una sola vez**.
No cobra sobre la mensualidad.

Es simple a propósito: el vendedor sabe exactamente cuánto gana el día que cierra,
y Manto conserva el ingreso recurrente, que es lo que sostiene la operación.

---

## 2. Cuánto se paga

**40 % del setup cobrado**, una sola vez. Un solo nivel, sin escala: la comisión
no cambia según el cliente ni según el tamaño del trato.

### El reparto del trabajo

| El vendedor | Manto |
|---|---|
| Consigue la reunión con el cliente. | Prepara la propuesta y el material a medida (página, mockup, flujo). |
| Muestra la demo que ya existe. | Lleva la reunión de negociación y cierra. |
| Registra y da seguimiento en la planilla. | Implementa, entrega y da soporte. |

El vendedor **no negocia precio ni cierra**. Su trabajo termina cuando la reunión
está agendada y el cliente vio la demo con interés real.

**Bono por volumen: S/ 300** al cerrar el **tercer cliente del trimestre**, y por
cada cliente adicional en ese mismo trimestre. Es un monto fijo, no un porcentaje:
premia el ritmo sin que la comisión se dispare justo en los tickets más grandes.

### Ejemplo (precios de ejemplo, reemplazar por los reales)

| | Setup | Mensualidad | Comisión 40 % | Le queda a Manto |
|---|---|---|---|---|
| Chatbot WhatsApp pyme | S/ 1,500 | S/ 300 | S/ 600 | S/ 900 + S/ 300/mes |
| Automatización n8n | S/ 2,500 | S/ 450 | S/ 1,000 | S/ 1,500 + S/ 450/mes |
| Web + asistente | S/ 3,500 | S/ 350 | S/ 1,400 | S/ 2,100 + S/ 350/mes |

> **Pendiente:** no hay precios de lista de Manto documentados en ningún lado.
> Esta tabla queda en ejemplo hasta que se fije el tarifario. Sin eso, el modelo
> no se puede cerrar (ver nota de margen en la sección 6).

### 2.1 Qué cuenta como reunión válida

Manto invierte horas construyendo el material a medida **antes** de cobrar. Por eso
una reunión que no va a ningún lado no es neutra: cuesta plata. Para que una
reunión active ese trabajo tiene que cumplir las cuatro:

1. **Con quien decide.** El dueño o el gerente que firma y aprueba el gasto — no un
   asistente, no un sobrino, no "se lo comento a mi jefe".
2. **Negocio real y activo**, con capacidad de pagar el ticket del servicio.
3. **El cliente sabe que es una reunión comercial.** No una conversada de favor.
4. **Ya vio la demo** y mostró interés concreto en algo puntual de su negocio.

Si la reunión no cumple, Manto igual asiste, pero **no construye material a medida**
hasta que califique. Una reunión caída o un no-show no cuentan como gestión.

Esto no es un castigo: es lo que permite que la comisión sea 40 % plano en vez de
escalonada. El filtro reemplaza a la escala.

---

## 3. Qué cuenta como venta del vendedor

**Registro previo.** Antes del primer contacto, el vendedor registra el lead en
la planilla (sección 5). Si no está registrado, no hay comisión: sin registro no
hay forma de resolver un conflicto de atribución después.

**Reglas:**

1. **Primero en registrar, gana.** Si dos vendedores registran al mismo cliente,
   la comisión es del primero.
2. **Vigencia 60 días.** Si el cliente no firma en 60 días desde el registro,
   el lead se libera. Si firma después con el mismo vendedor activo en la cuenta,
   se respeta la comisión; si firma por gestión de otro, es del otro.
3. **Clientes ya en el pipeline de Manto no cuentan.** Cualquiera que ya haya
   tenido contacto comercial con Manto antes del registro queda excluido. La
   planilla de leads existentes es la referencia.
4. **Venta cerrada = dinero en la cuenta.** No cuenta la propuesta aceptada, ni
   el "sí" por WhatsApp, ni la factura emitida. Cuenta el pago recibido.

**Referidos del propio cliente:** si un cliente traído por el vendedor refiere a
otro, ese segundo cliente también es del vendedor, con las mismas reglas.

---

## 4. Cuándo y cómo se paga

- **Base de cálculo:** el setup **efectivamente cobrado**, neto de descuentos.
  Si se aplica un descuento, la comisión baja en la misma proporción.
- **Plazo:** dentro de los **15 días calendario** después de que el pago del
  cliente esté en la cuenta de Manto **y** haya pasado el período de garantía.
- **Período de garantía: 30 días** desde el pago. Si el cliente pide devolución
  o hace contracargo dentro de ese plazo, no hay comisión; si ya se pagó, se
  descuenta de la siguiente (*clawback*).
- **Pagos en cuotas:** si el cliente paga el setup en partes, la comisión se paga
  en la misma proporción, a medida que entra cada cuota.
- **Bono por volumen:** se paga junto con la comisión del cliente que lo gatilla,
  y solo si esa venta pasó su período de garantía.
- **Formalidad:** el vendedor es independiente, no hay vínculo laboral, y emite
  recibo por honorarios por cada comisión. *Confirmar con un contador el
  tratamiento de renta de 4.ª categoría y la retención que corresponda antes de
  pagar la primera comisión.*

**Lo que el vendedor no hace, nunca:**

- **Hablar de precio.** Ni rangos, ni "debe salir como…", ni descuentos. El precio
  se pone en la reunión de negociación, y esa la lleva Manto.
- Prometer funcionalidades, integraciones, plazos de entrega o garantías.
- Firmar en nombre de Manto.

Si el cliente pregunta el precio en la demo, la respuesta es que depende del
alcance y que eso se ve en la reunión con el equipo. Adelantar una cifra quema el
margen antes de que empiece la negociación.

Un alcance prometido y no aprobado lo asume comercialmente quien lo prometió: se
descuenta de su comisión el costo de cumplirlo.

---

## 5. Planilla de seguimiento

Una sola hoja compartida, una fila por lead. Campos mínimos:

| Campo | Notas |
|---|---|
| `fecha_registro` | Marca la vigencia de 60 días y decide la atribución. |
| `vendedor` | |
| `empresa` / `contacto` / `telefono` | |
| `origen` | Cómo llegó (conocido, frío, referido de cliente X). |
| `estado` | registrado · reunión · propuesta · cerrado · perdido · vencido |
| `servicio` | Chatbot / automatización / web / mixto |
| `setup_acordado` · `mensualidad` | |
| `descuento` | Reduce la base de comisión. |
| `fecha_reunion` | |
| `quien_asistio` | Nombre y cargo. Es lo que define si la reunión califica (criterio 1 de 2.1). |
| `reunion_calificada` | Sí / no, según los 4 criterios de 2.1. Decide si se construye material a medida. |
| `fecha_cobro` | Arranca el reloj de la garantía. |
| `fecha_pago_comision` · `monto_comision` | |
| `horas_preventa` | Horas invertidas en material a medida antes del cierre. Alimenta el cálculo de margen. |
| `notas` | Cómo llegó la reunión, quién estuvo, qué pidió el cliente. |

Esto vive bien en el Supabase `manto-demo` una vez que se reactive; mientras
tanto, una hoja de cálculo compartida basta y es más rápido de operar.

---

## 6. INTERNO — no compartir con vendedores

### 6.1 Riesgo de margen

**40 % plano es la tarifa alta aplicada a todas las ventas.** Vale tenerlo
consciente: el vendedor consigue la reunión y demea, pero la negociación, el
cierre y la entrega los hace Manto. En la escala anterior ese reparto pagaba
30 %; el 40 % es una decisión deliberada de pagar por encima, porque conseguir
reuniones en frío es la parte más difícil de tercerizar y no hay a quién más
pedírsela hoy. La contrapartida es el filtro de reunión válida (2.1): sin escala,
el filtro es lo único que separa una gestión buena de una mala.

Con 40 %, en el ejemplo del chatbot (setup S/ 1,500) quedan S/ 900 para cubrir la
implementación, más la mensualidad íntegra. Al 60 % quedaban S/ 600, que
probablemente no cubren las horas.

### 6.2 El costo nuevo: preventa

El modelo de negociar con la página o el mockup ya construido **mueve horas de
Manto antes de que exista un pago**. Ese costo no está en ningún número de este
documento y es el que más rápido se puede ir de las manos:

- Una reunión que no cierra ahora cuesta **horas de build**, no solo tiempo de
  reunión. A 3 reuniones perdidas por cliente ganado, el costo de adquisición
  real puede superar la comisión.
- Por eso el campo `horas_preventa` en la planilla y la regla de no construir a
  medida hasta que la reunión califique. Sin esos dos, no hay forma de saber si
  el modelo pierde plata.
- **Métrica a mirar desde el cliente 1:** reuniones calificadas por cierre. Si
  pasa de 4 a 1, el problema no es la comisión, es la calificación de leads —
  hay que apretar el filtro, no bajar el %.

Regla práctica mientras no haya datos: el material a medida se hace **acotado**
(una página, un flujo, no un producto), con techo de horas por reunión. Si un
prospecto pide más que eso antes de firmar, ya no es preventa, es trabajo — y se
cobra un anticipo.

### 6.3 Cuándo revisar

Calcular para cada servicio el **costo real de implementación** (horas + APIs +
hosting) **más las horas de preventa promedio**, y en cuántos meses de mensualidad
se recupera la comisión. Si el recupero pasa de **4-5 meses**, la salida no es
bajar el 40 % (ya está comprometido y bajarlo después quema la relación): es
subir el precio del setup o apretar el filtro de 2.1.

No hace falta reajustar nada antes de lanzar — con 2-3 vendedores y pocos cierres
el riesgo es acotado y se aprende rápido. Pero revisar los números **antes** del
quinto cliente, no después.

**Si un vendedor pide más:** el margen para negociar no está en el porcentaje del
setup, está en el bono por volumen (subirlo a S/ 400-500) o en un residual chico
sobre la mensualidad de los clientes que él mismo retiene. Subir el % del setup es
lo único que pega directo contra el costo de entregar.

### 6.4 Entrada del socio (Fase 2)

Regla que conviene fijar desde ahora, porque evita el conflicto más previsible:

> **El porcentaje del vendedor no cambia cuando entra el socio.** Lo que se
> reparte distinto es el resto, entre Arian y el socio.

Es decir, el vendedor siempre ve el mismo número (40 % del setup). Si mañana
el socio entra a la cuenta, el vendedor no cobra menos: se ajusta el reparto
interno del 60 % restante. Si el vendedor percibe que su comisión baja porque
"entró alguien más", se quema la relación — y son amigos.

Para repartir el resto, el criterio que menos discusión genera es **quién entrega
el trabajo**, no quién reclutó al vendedor. Reclutar al vendedor es un aporte de
una vez; entregar el proyecto es un costo recurrente. Fijar ese criterio con el
socio **antes** de que el primer cliente de un vendedor caiga en su cancha.

**Fecha de revisión de todo este documento: a los 90 días o al quinto cliente
cerrado, lo que ocurra primero.**

---

## Pendientes antes de lanzar

- [ ] Fijar tarifario real de Manto (setup y mensualidad por servicio).
- [ ] Calcular costo de implementación y meses de recupero por servicio (6.1).
- [ ] Acordar con el socio el criterio de reparto del resto (6.2).
- [ ] Confirmar con contador el tratamiento de recibos por honorarios.
- [ ] Crear la planilla y cargar los clientes ya en pipeline (regla 3.3).
- [ ] Entregar a cada vendedor: secciones 1-5, demos de `acm02-a/demos` y guion de venta.
