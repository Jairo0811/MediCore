from pathlib import Path

p = Path('README.md')
s = p.read_text(encoding='utf-8')

old = '''La base funcional utilizada para `DispensarioMedicoUnapec` proviene de un documento de **Proyecto Final de UNAPEC de 2015**, identificado en el material original como **Profesor: Juan P. Valdez**. Dicho documento plantea el desarrollo de un sistema para el Dispensario Médico de UNAPEC con gestión de tipos de fármacos, marcas, ubicaciones, medicamentos, pacientes, médicos, registro de visitas, consultas por criterios y reportes de visitas. La propuesta original especificaba su implementación con la versión vigente de **.NET Framework utilizando WinForms**.

MediCore conserva ese problema de negocio como **antecedente académico y funcional**, pero lo reinterpreta con una arquitectura moderna, frontend y backend desacoplados, tecnologías actuales y un alcance considerablemente ampliado.'''

new = '''La base funcional utilizada para `DispensarioMedicoUnapec` proviene de una presentación de **Proyecto Final de Universidad APEC de 2020**, identificada explícitamente como **Profesor: Juan P. Valdez**. El enunciado propone un sistema para el Dispensario Médico de UNAPEC con gestión de tipos de fármacos, marcas, ubicaciones, medicamentos, pacientes, médicos, registro de visitas, consultas por criterios y reportes de visitas. El documento solicita desarrollar la solución con una **tecnología Open Source de preferencia bajo el patrón MVC**.

MediCore conserva ese problema de negocio como **antecedente académico y funcional**, pero lo reinterpreta con una arquitectura moderna, frontend y backend desacoplados, tecnologías actuales y un alcance considerablemente ampliado.'''

if old not in s:
    raise SystemExit('Antecedent block not found')
s = s.replace(old, new, 1)

anchor = '### 🏫 Cruce institucional ITLA → UNAPEC'
lineage = '''### 📚 Línea académica de Juan P. Valdez

MediCore también pertenece a una **línea académica común de enunciados de Proyecto Final elaborados por el profesor Juan P. Valdez en 2020**. Dentro de esta colección se han identificado tres problemas de negocio que posteriormente dieron origen o sirvieron como base académica para proyectos del portafolio: **Dispensario Médico → MediCore**, **Video Club → CineGest** y **Rentcar → RentCarRD**.

| Orden | Enunciado académico de 2020 | Evolución en el portafolio | Relación con Juan P. Valdez |
|---:|---|---|---|
| 1 | Dispensario Médico de UNAPEC | **MediCore** | Enunciado de Proyecto Final elaborado por **Juan P. Valdez** |
| 2 | Sistema de Video Club | [**CineGest**](https://github.com/Jairo0811/CineGest) | Enunciado de Proyecto Final elaborado por **Juan P. Valdez** |
| 3 | Sistema de Rentcar | [**RentCarRD**](https://github.com/Jairo0811/RentCarRD) | Enunciado de Proyecto Final elaborado por **Juan P. Valdez** |

Esta relación se documenta como **continuidad por origen del enunciado académico**. No significa que Juan P. Valdez haya sido el profesor efectivo de MediCore y CineGest durante sus implementaciones de 2026; en esos proyectos el docente fue **Ing. Omar Antonio De Jesus De La Cruz Gonzalez**. La línea común corresponde al origen de los requerimientos académicos.

'''

if anchor not in s:
    raise SystemExit('ITLA anchor not found')
s = s.replace(anchor, lineage + anchor, 1)

p.write_text(s, encoding='utf-8')
