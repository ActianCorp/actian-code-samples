import os
import sys

from sqlalchemy import (
    CHAR,
    Column,
    ForeignKey,
    Integer,
    String,
    create_engine,
    delete,
    update,
)
from sqlalchemy.orm import Session, declarative_base


def get_connection_url():
    if "DEMO_DB" in os.environ:
        db = os.getenv("DEMO_DB")
    else:
        sys.exit("Please set environment variable DEMO_DB to the name of an existing database.")

    if "DEMO_ID" in os.environ and "DEMO_PW" in os.environ:
        id = os.getenv("DEMO_ID")
        pw = os.getenv("DEMO_PW")
    else:
        id = ""
        pw = ""

    if id and pw:
        creds = id + ":" + pw + "@"
    else:
        creds = ""

    url = "ingres://" + creds + "/" + db

    return url


# Main

Base = declarative_base()


class Person(Base):
    __tablename__ = "employees"
    ssn = Column("ssn", Integer, primary_key=True)
    fname = Column("fname", String(15))
    lname = Column("lname", String(15))
    gender = Column("gender", CHAR)
    age = Column("age", Integer)

    def __init__(self, ssn, fname, lname, gender, age):
        self.ssn = ssn
        self.fname = fname
        self.lname = lname
        self.gender = gender
        self.age = age

    def __repr__(self):
        return f"({self.ssn}) {self.fname} {self.lname} ({self.gender},{self.age})"


print("Creating connection")
url = get_connection_url()
engine = create_engine(url, echo=False)

print("Dropping and recreating tables")
Base.metadata.drop_all(bind=engine)
Base.metadata.create_all(bind=engine)

session = Session(engine)

person1 = Person(12345, "Stu", "Green", "M", 41)
person2 = Person(54321, "Ray", "White", "M", 17)
person3 = Person(67890, "Bob", "Blue", "M", 83)
person4 = Person(98765, "Jim", "Black", "M", 55)
person5 = Person(13579, "Liz", "Brown", "F", 29)

print("Adding rows")
session.add(person1)
session.add(person2)
session.add(person3)
session.add(person4)
session.add(person5)

session.commit()

results = session.query(Person).all()
print(results)

print("Updating 1 row")
stmt = update(Person).where(Person.ssn == 54321).values(lname="Plum")
session.execute(stmt)

session.commit()

results = session.query(Person).all()
print(results)

print("Deleting 1 row")
stmt = delete(Person).where(Person.ssn == 98765)
session.execute(stmt)

session.commit()

results = session.query(Person).all()
print(results)
