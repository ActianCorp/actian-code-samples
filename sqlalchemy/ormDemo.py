# Example SQLAlchemy ORM program that uses the Ingres dialect.
#
# The code creates table "Employees" (drops/recreates if exists),
#  adds rows to the table, updates a row, deletes a row,
#  queries and prints the rows after each operation.
#
# Environment variables:
#   SQLALCHEMY_INGRES_ODBC_DRIVER_NAME - Name of Ingres ODBC driver, required
#   DEMO_DB - Name of database to use, required
#   DEMO_ID - Userid for connecting, optional
#   DEMO_PW - Password for connecting, optional
#   Note: DEMO_ID and DEMO_PW will only be used if both are set.
#
# Code is partly modeled after the tutorial in video:
#   "SQLAlchemy Turns Python Objects Into Database Entries"
#    by NeuralNine at:  https://www.youtube.com/watch?v=AKQ3XEDI9Mw
#
# Code tested with these Python packages:
#   SQLAlchemy        2.0.25
#   ingres_sa_dialect 0.4
#   pypyodbc          1.3.6
#   setuptools        63.2.0

import os, sys
from sqlalchemy import create_engine, ForeignKey, Column, String, Integer, CHAR, update, delete
from sqlalchemy.orm import Session, declarative_base

def get_connection_url():

    if "DEMO_DB" in os.environ:
        db = os.getenv("DEMO_DB")
    else:
        print("Please set environment variable DEMO_DB to the name of an existing database.")
        sys.exit()

    if ( "DEMO_ID" in os.environ and "DEMO_PW" in os.environ ):
        id = os.getenv("DEMO_ID")
        pw = os.getenv("DEMO_PW")
    else:
        id = "" 
        pw = ""

    if ( id and pw ):
        creds = id + ":" + pw + "@"
    else:
        creds = ""

    url = "ingres://"+creds+"/"+db

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
stmt = update(Person).where(Person.ssn==54321).values(lname="Plum")
session.execute(stmt)

session.commit()

results = session.query(Person).all()
print(results)

print("Deleting 1 row")
stmt = delete(Person).where(Person.ssn==98765)
session.execute(stmt)

session.commit()

results = session.query(Person).all()
print(results)

