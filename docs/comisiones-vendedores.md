# Comisiones de vendedores — Manto

**Estado:** borrador v1 · Fase 1 (solo Arian) · Fecha: 2026-08-13
**Aplica a:** vendedores externos que traen clientes a Manto.

Este documento define cómo se paga a un vendedor. Las secciones 1 a 5 son las
que se comparten con el vendedor. La sección 6 es **interna** — no se comparte.

---

## 1. El modelo en una línea

El vendedor cobra **un porcentaje del pago inicial (setup) del cliente, una sola
vez**. No cobra sobre la mensualidad.

Es simple a propósito: el vendedor sabe exactamente cuánto gana el día que cierra,
y Manto conserva el ingreso recurrente, que es lo que sostiene la operación.

---

## 2. Cuánto se paga

Base **40 %** del setup cobrado. Sube por desempeño, no por antigüedad:

| Concepto | % | Cuándo aplica |
|---|---|---|
| Base | 40 % | El vendedor trae el lead calificado, pero Arian entra a vender: hace la demo, negocia o rescata el cierre. |
| + 10 % | 50 % | El vendedor cierra solo. Arian entra únicamente para el alcance técnico y la firma. |
| + 10 % | 60 % | Además, es el **tercer cliente cerrado del vendedor en el trimestre** (del 3.º en adelante, incluido). |

Máximo **60 %**. El nivel se evalúa **por cliente**, y se congela al momento del
cierre — no se recalcula después.

### Ejemplo (precios de ejemplo, reemplazar por los reales)

| | Setup | Mensualidad | Comisión 40 % | Comisión 50 % | Comisión 60 % |
|---|---|---|---|---|---|
| Chatbot WhatsApp pyme | S/ 1,500 | S/ 300 | S/ 600 | S/ 750 | S/ 900 |
| Automatización n8n | S/ 2,500 | S/ 450 | S/ 1,000 | S/ 1,250 | S/ 1,500 |
| Web + asistente | S/ 3,500 | S/ 350 | S/ 1,400 | S/ 1,750 | S/ 2,100 |

> **Pendiente:** no hay precios de lista de Manto documentados en ningún lado.
> Esta tabla queda en ejemplo hasta que se fije el tarifario. Sin eso, el modelo
> no se puede cerrar (ver nota de margen en la sección 6).

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
- **Formalidad:** el vendedor es independiente, no hay vínculo laboral, y emite
  recibo por honorarios por cada comisión. *Confirmar con un contador el
  tratamiento de renta de 4.ª categoría y la retención que corresponda antes de
  pagar la primera comisión.*

**Lo que el vendedor no puede hacer sin aprobación escrita de Arian:**

- Ofrecer descuentos, plazos de pago o garantías distintas a las de la propuesta.
- Prometer funcionalidades, integraciones o fechas de entrega.
- Firmar en nombre de Manto.

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
| `nivel_comision` | 40 / 50 / 60, congelado al cierre. |
| `fecha_cobro` | Arranca el reloj de la garantía. |
| `fecha_pago_comision` · `monto_comision` | |
| `notas` | Quién cerró, si Arian entró a vender (define 40 vs 50). |

Esto vive bien en el Supabase `manto-demo` una vez que se reactive; mientras
tanto, una hoja de cálculo compartida basta y es más rápido de operar.

---

## 6. INTERNO — no compartir con vendedores

### 6.1 Riesgo de margen

Pagar 40-60 % del setup solo es sostenible si el setup tiene margen real sobre
el costo de implementar. Si el setup está puesto más o menos al costo de las
horas de implementación (que es lo habitual al arrancar), un 60 % deja el primer
mes en pérdida y esa pérdida se recupera con la mensualidad.

Antes de comprometer estos porcentajes con los amigos, calcular para cada
servicio: **costo real de implementación** (horas + APIs + hosting) y en cuántos
meses de mensualidad se recupera la comisión. Si el recupero pasa de **4-5
meses**, bajar la base a 30-35 % o mover parte de la comisión al segundo pago del
cliente. Con churn alto y comisión al 60 %, cada venta cuesta plata.

No hace falta reajustar la escala antes de lanzar — con 2-3 vendedores y pocos
cierres el riesgo es acotado y se aprende rápido. Pero revisar los números
**antes** del quinto cliente, no después.

### 6.2 Entrada del socio (Fase 2)

Regla que conviene fijar desde ahora, porque evita el conflicto más previsible:

> **El porcentaje del vendedor no cambia cuando entra el socio.** Lo que se
> reparte distinto es el resto, entre Arian y el socio.

Es decir, el vendedor siempre ve el mismo número (40-60 % del setup). Si mañana
el socio entra a la cuenta, el vendedor no cobra menos: se ajusta el reparto
interno del 40-60 % restante. Si el vendedor percibe que su comisión baja porque
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
