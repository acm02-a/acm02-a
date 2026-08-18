# ¡Hola! Soy Arian Casas 👋

Estudiante de **Ingeniería de Sistemas** (UPC, Lima, Perú) y co-fundador de
**Manto**, una agencia de automatización con IA y desarrollo web para pymes.
Me gusta construir cosas que trabajan solas: pipelines de datos, bots y
automatizaciones que corren mientras duermo.

## 🔧 Lo que uso

`Python` · `SQL` · `DuckDB` · `pandas` · `scikit-learn` · `FastAPI` · `Streamlit` · `n8n` · `Supabase` · `Docker` · `GitHub Actions` · `JavaScript`

## 📌 Proyectos destacados

Los tres primeros cuentan una sola historia: encontré un problema en cómo el
Estado publica sus datos, construí el almacén que lo corrige, el modelo que
anticipa el riesgo y el asistente que hace la información accesible.

| Proyecto | Qué hace |
|---|---|
| [compras-peru](https://github.com/acm02-a/compras-peru) | **Analítica de las compras públicas del Perú**: 303 465 contratos en un star schema sobre DuckDB, 35 reglas de calidad de datos y un tablero de riesgo. Un test de unicidad de grano reveló que el portal del Estado publica una fila por *modificación* y no por contrato: sumar sin corregirlo infla el gasto 2,7x. **[Ver el reporte →](https://acm02-a.github.io/compras-peru/)** |
| [riesgo-contratos](https://github.com/acm02-a/riesgo-contratos) | **Machine learning sin fuga temporal**: predice qué contratos terminarán modificados usando solo lo que se sabe el día de la firma. Partición temporal estricta y comparación contra baselines. De los 100 contratos que marca, 59 se modificaron (3,3x mejor que al azar). **[Ver los resultados →](https://acm02-a.github.io/riesgo-contratos/)** |
| [tramites-rag](https://github.com/acm02-a/tramites-rag) | **RAG con evaluación medida**: asistente sobre 116 fichas de trámites de gob.pe que responde citando la ficha oficial. Lo que casi nadie muestra: 50 preguntas con respuesta conocida que miden si el sistema *realmente* encuentra la información. Diagnosticar los fallos subió el recall@5 de 0,62 a 0,68. |
| [flujo](https://github.com/acm02-a/flujo) | **Motor de workflows construido desde cero** (un mini-n8n): pipelines en YAML con DAG, reintentos con backoff, plantillas, condiciones seguras sin `eval` y pasos de IA. Código de infraestructura, no solo de aplicación. |
| [factura-ai](https://github.com/acm02-a/factura-ai) | **Procesamiento inteligente de facturas (IDP)**: extracción con IA + validación de negocio real (RUC con dígito verificador, IGV 18%) + decisión automática con human-in-the-loop. La IA con guardrails de producción. |
| [macro-peru-analytics](https://github.com/acm02-a/macro-peru-analytics) | **Data analytics de punta a punta**: 26 años de datos macro del BCRP → SQLite → consultas SQL (window functions, CTEs) → notebook con hallazgos. ¿Sabías que el sol vale más hoy que en el 2000? |

Todos con CI (lint + tests en cada push) y funcionando en producción real.
También mantengo [pen-radar](https://github.com/acm02-a/pen-radar) (pipeline del
dólar que se actualiza solo), [lead-bot](https://github.com/acm02-a/lead-bot)
(API que captura leads y responde con IA) y
[csv-cleaner](https://github.com/acm02-a/csv-cleaner) (CLI que limpia CSVs
sucios con reporte auditable).

## 💼 En Manto

Automatizaciones con n8n, chatbots de WhatsApp y webs para pymes peruanas.
Si tienes un proceso repetitivo, probablemente se puede automatizar.

## 📫 Contacto

- ✉️ ariancasas1@gmail.com
- 💼 [LinkedIn](https://www.linkedin.com/in/arian-casas)
