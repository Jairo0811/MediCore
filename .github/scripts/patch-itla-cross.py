from pathlib import Path

path = Path("README.md")
text = path.read_text(encoding="utf-8")

anchor = '''Vistos en conjunto, estos proyectos muestran continuidad tanto en la relación docente como en la colaboración entre compañeros. La identidad de la compañera recurrente se considera verificada por coincidencia de **nombre completo y matrícula A00110380**.

## 🧩 Dominios
'''

replacement = '''Vistos en conjunto, estos proyectos muestran continuidad tanto en la relación docente como en la colaboración entre compañeros. La identidad de la compañera recurrente se considera verificada por coincidencia de **nombre completo y matrícula A00110380**.

### 🏫 Cruce institucional ITLA → UNAPEC

MediCore también documenta un cruce institucional entre **ITLA** y **UNAPEC**. Además de Francis Jairo Matías Rosario, **José Samuel Peña Acevedo**, integrante del equipo académico original de MediCore, también cursó estudios previamente en el **Instituto Tecnológico de Las Américas (ITLA)** antes de coincidir posteriormente en este proyecto de UNAPEC.

Este vínculo se documenta como una **trayectoria institucional compartida**, no como continuidad por asignaturas cursadas conjuntamente en ITLA. Con la información disponible no se establece que Francis Jairo Matías Rosario y José Samuel Peña Acevedo hayan coincidido en una misma materia durante su etapa en esa institución; la coincidencia académica confirmada corresponde a **MediCore (ISO-605, Enero - Abril 2026)** en UNAPEC.

| Integrante | Matrícula UNAPEC | Matrícula ITLA | Relación documentada |
|---|---|---|---|
| **Francis Jairo Matías Rosario** | A00115261 | **2015-2984** | ITLA → UNAPEC |
| **José Samuel Peña Acevedo** | A00107391 | **2017-4611** | ITLA → UNAPEC; coincidencia posterior en MediCore |

Este cruce añade una dimensión adicional a la continuidad académica del repositorio: estudiantes con formación previa en ITLA que posteriormente convergen en un mismo proyecto académico de Ingeniería de Software en UNAPEC.

## 🧩 Dominios
'''

if anchor not in text:
    raise SystemExit("Continuity anchor not found")

path.write_text(text.replace(anchor, replacement, 1), encoding="utf-8")
