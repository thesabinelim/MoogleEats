use rand::{distributions::DistString, prelude::Distribution, Rng};

const EXTERNAL_ID_LEN: usize = 12;
const CROCKFORD_BASE32_ENCODING: &'static [u8] = b"0123456789ABCDEFGHJKMNPQRSTVWXYZ";

pub fn gen_external_id<R>(rng: &mut R) -> String
where
    R: Rng,
{
    CrockfordBase32.sample_string(rng, EXTERNAL_ID_LEN)
}

struct CrockfordBase32;

impl Distribution<u8> for CrockfordBase32 {
    fn sample<R: Rng + ?Sized>(&self, rng: &mut R) -> u8 {
        let n = rng.gen_range(0..CROCKFORD_BASE32_ENCODING.len());
        CROCKFORD_BASE32_ENCODING[n]
    }
}

impl DistString for CrockfordBase32 {
    fn append_string<R: Rng + ?Sized>(&self, rng: &mut R, string: &mut String, len: usize) {
        string.push_str(
            String::from_utf8(self.sample_iter(rng).take(len).collect::<Vec<u8>>())
                .expect("Encountered invalid UTF-8 character")
                .as_str(),
        );
    }
}
