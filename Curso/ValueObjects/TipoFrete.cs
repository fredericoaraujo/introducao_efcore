namespace CursoEFCore.ValueObjects;

public enum TipoFrete
{
    CIF, // quando o remetente paga o frete
    FOB, // destinatário paga o frete
    SemFrete // quando cliente retira na loja
}
