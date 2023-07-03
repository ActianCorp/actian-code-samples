
use odbc_api::{ConnectionOptions, Environment, buffers::TextRowSet, Cursor, buffers::{ColumnarBuffer, TextColumn}};
use anyhow::Error;
use clap::{Parser};
use std::str;


#[derive(Parser)]
struct CliOpts {
    #[clap(short, long)]
    connection_string : String,
}
fn main() -> Result<(), Error> {
    let env = Environment::new()?;
    let opts = CliOpts::parse();    
    let conn = env.connect_with_connection_string(&opts.connection_string, ConnectionOptions::default())?;
    let mut cursor = conn.execute("select cap_capability, cap_value from iidbcapabilities", ())?
                         .expect("executing query failed");
    let buffer = TextRowSet::for_cursor(1, &mut cursor, Some(4000))?;
    let mut cursor = cursor.bind_buffer(buffer)?;

    fn get_val(colidx : usize, rowidx : usize, batch : &ColumnarBuffer<TextColumn<u8>> ) -> &str {
        batch.at(colidx, rowidx)
            .map_or("", |v| {
                str::from_utf8(v)
                    .unwrap_or("cannot convert to string")
            })
    }

    while let Some(batch) = cursor.fetch()? {
            // iterate over every row - useful only if the batch size for textrowset cursor is set > 1
            for row_index in 0..batch.num_rows() { 
                let  cap = get_val(0, row_index, batch);
                let  capval = get_val(1, row_index, batch);
                println!("[{cap} - {capval}]");
        }
    }
    Ok(())
}
