CREATE PROCEDURE get_airlines_by_country( IN country_param CHAR(2) NOT NULL) RESULT ROW (CHAR(60)) AS
  DECLARE airline_name CHAR(60);
    BEGIN
      FOR
        select al_name into :airline_name from airline where al_ccode = :country_param
        DO RETURN ROW (:airline_name);
      ENDFOR;
    END; \g

COMMIT \g

