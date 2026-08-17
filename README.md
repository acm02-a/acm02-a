# ¡Hola! Soy Arian Casas 👋

Estudiante de **Ingeniería de Sistemas** (UPC, Lima, Perú) y co-fundador de
**Manto**, una agencia de automatización con IA y desarrollo web para pymes.
Me gusta construir cosas que trabajan solas: pipelines de datos, bots y
automatizaciones que corren mientras duermo.

## 🔧 Lo que uso

`Python` · `SQL` · `DuckDB` · `pandas` · `FastAPI` · `Streamlit` · `n8n` · `Supabase` · `Docker` · `GitHub Actions` · `JavaScript`

## 📌 Proyectos destacados

| Proyecto | Qué hace |
|---|---|
| [compras-peru](https://github.com/acm02-a/compras-peru) | **Analítica de las compras públicas del Perú**: 303 465 contratos modelados en un star schema sobre DuckDB, con 35 reglas de calidad de datos y un tablero de riesgo. Un test de unicidad de grano reveló que el portal del Estado publica una fila por *modificación* y no por contrato: sumar sin corregirlo infla el gasto 2,7x. **[Ver el reporte →](https://acm02-a.github.io/compras-peru/)** |
| [flujo](https://github.com/acm02-a/flujo) | **Motor de workflows construido desde cero** (un mini-n8n): pipelines en YAML con DAG, reintentos con backoff, plantillas, condiciones seguras sin `eval` y pasos de IA. Código de infraestructura, no solo de aplicación. |
| [factura-ai](https://github.com/acm02-a/factura-ai) | **Procesamiento inteligente de facturas (IDP)**: extracción con IA + validación de negocio real (RUC con dígito verificador, IGV 18%) + decisión automática con human-in-the-loop. La IA con guardrails de producción. |
| [macro-peru-analytics](https://github.com/acm02-a/macro-peru-analytics) | **Data analytics de punta a punta**: 26 años de datos macro del BCRP → SQLite → consultas SQL (window functions, CTEs) → notebook con hallazgos. ¿Sabías que el sol vale más hoy que en el 2000? |
| [pen-radar](https://github.com/acm02-a/pen-radar) | Dashboard del dólar (USD/PEN) que se **actualiza solo** cada día con datos oficiales del BCRP: pipeline de datos + GitHub Actions que reescribe su propio README y publica un JSON consumible como API. |
| [lead-bot](https://github.com/acm02-a/lead-bot) | API en FastAPI que captura leads desde una landing y **responde con IA** (Groq) o con respaldo por plantilla. SQLite, rate limiting, Docker y tests. |

Todos con CI (lint + tests en cada push) y funcionando en producción real.
También mantengo [csv-cleaner](https://github.com/acm02-a/csv-cleaner), una CLI
instalable con pip que limpia CSVs sucios y entrega un reporte auditable de cada
cambio.

## 💼 En Manto

Automatizaciones con n8n, chatbots de WhatsApp y webs para pymes peruanas.
Si tienes un proceso repetitivo, probablemente se puede automatizar.

## 📫 Contacto

- ✉️ ariancasas1@gmail.com
- 💼 [LinkedIn](https://www.linkedin.com/in/arian-casas)
