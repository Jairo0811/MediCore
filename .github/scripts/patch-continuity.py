from pathlib import Path
import re

path = Path("README.md")
text = path.read_text(encoding="utf-8")

section = """## 🧭 Continuidad académica

**MediCore** ocupa un punto central dentro de la continuidad académica documentada en estos proyectos de UNAPEC, porque conecta dos tipos de coincidencia verificable: **profesor recurrente** y **compañera recurrente**. Estas relaciones son **formativas y cronológicas**; no implican dependencias técnicas entre las aplicaciones.

### 👨‍🏫 Continuidad por profesor

Durante **Enero - Abril de 2026**, el profesor **Ing. Omar Antonio De Jesus De La Cruz Gonzalez** impartió dos asignaturas en las que Francis Jairo Matías Rosario participó en proyectos finales distintos: **Desarrollo de Software con Tecnología Propietaria 1 (ISO-605)** con MediCore y **Desarrollo de Software con Tecnología Open Source 1 (ISO-610)** con [CineGest](https://github.com/Jairo0811/CineGest).

| Orden | Código | Asignatura | Proyecto | Período | Vínculo |
|---:|---|---|---|---|---|
| 1 | ISO-605 | Desarrollo de Software con Tecnología Propietaria 1 | **MediCore** | Enero - Abril 2026 | Mismo profesor |
| 2 | ISO-610 | Desarrollo de Software con Tecnología Open Source 1 | [**CineGest**](https://github.com/Jairo0811/CineGest) | Enero - Abril 2026 | Mismo profesor |

La coincidencia documenta una experiencia paralela con el mismo docente en dos líneas complementarias del plan de estudios: **tecnología propietaria** y **tecnología open source**.

### 👥 Continuidad por compañera

MediCore también inicia una secuencia académica con **Emely Marie Castillo Rivera (A00110380)**. Emely formó parte del equipo de MediCore en **ISO-605** durante **Enero - Abril de 2026** y volvió a coincidir con Francis Jairo Matías Rosario en [**EcoSoft**](https://github.com/Jairo0811/Ecosoft), proyecto de **Proyecto de Software 1 (ISO-705)** desarrollado en **Mayo - Agosto de 2026**.

| Orden | Código | Asignatura | Proyecto | Período | Compañera recurrente |
|---:|---|---|---|---|---|
| 1 | ISO-605 | Desarrollo de Software con Tecnología Propietaria 1 | **MediCore** | Enero - Abril 2026 | **Emely Marie Castillo Rivera — A00110380** |
| 2 | ISO-705 | Proyecto de Software 1 | [**EcoSoft**](https://github.com/Jairo0811/Ecosoft) | Mayo - Agosto 2026 | **Emely Marie Castillo Rivera — A00110380** |

Vistos en conjunto, estos proyectos muestran continuidad tanto en la relación docente como en la colaboración entre compañeros. La identidad de la compañera recurrente se considera verificada por coincidencia de **nombre completo y matrícula A00110380**."""

updated = re.sub(
    r"## 🔗 Continuidad académica.*?(?=\n\n## 🧩 Dominios)",
    section,
    text,
    flags=re.S,
)
if updated == text:
    raise SystemExit("Continuity block not found")
path.write_text(updated, encoding="utf-8")
